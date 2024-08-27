"use strict";

var connection = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub")
    .configureLogging(signalR.LogLevel.Debug)
    .build();

connection.on("ReceiveMessage", function (user, message, timestamp, profilePicture) {
    console.log(`Received message from ${user} at ${timestamp}: ${message}`); // Debugging line
    if (!user || !message || !timestamp) {
        console.error("Received invalid message:", { user, message, timestamp });
        return;
    }

    // Convert the timestamp to a human-readable format
    var date = new Date(timestamp);
    if (!isNaN(date.getTime())) { // Check if the date is valid
        var options = { day: '2-digit', month: 'short', year: 'numeric', hour: '2-digit', minute: '2-digit', hour12: false };
        timestamp = date.toLocaleDateString('en-GB', options);
    } else {
        console.error("Invalid timestamp format:", timestamp);
        timestamp = "Invalid time";
    }

    var li = document.createElement("li");
    li.className = "list-group-item d-flex align-items-start";

    // Profile picture
    var img = document.createElement("img");
    img.src = profilePicture || "/images/default-profile.png"; // Fallback image if profilePicture is empty
    img.alt = `${user}'s profile picture`;

    // Message content container
    var messageContainer = document.createElement("div");

    var userSpan = document.createElement("strong");
    userSpan.textContent = `@${user}:`;

    var messageContent = document.createElement("p");
    messageContent.className = "message-content mb-0";
    messageContent.textContent = message;

    var messageTimestamp = document.createElement("small");
    messageTimestamp.className = "message-timestamp";
    messageTimestamp.textContent = timestamp;

    messageContainer.appendChild(userSpan);
    messageContainer.appendChild(messageContent);
    messageContainer.appendChild(messageTimestamp);

    li.appendChild(img);
    li.appendChild(messageContainer);

    document.getElementById("messagesList").appendChild(li);

    var messagesList = document.getElementById("messagesList");
    if (messagesList.children.length > 10) {
        messagesList.scrollTop = messagesList.scrollHeight;
    }
});

connection.start().then(function () {
    console.log("SignalR connection established."); // Debugging line
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    console.error("SignalR connection error:", err.toString()); // Debugging line
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    sendMessage();
    event.preventDefault();
});

document.getElementById("messageInput").addEventListener("keypress", function (event) {
    if (event.key === "Enter") {
        sendMessage();
        event.preventDefault(); // Prevent default form submission behavior
    }
});

document.getElementById("recipientUserId").addEventListener("change", function () {
    var recipientUserId = this.value;
    var messagesList = document.getElementById("messagesList");

    if (recipientUserId === "") {
        // Clear the chat history when the blank option is selected
        messagesList.innerHTML = "";
        return; // Exit early as no need to fetch chat history
    }

    fetch(`/Users/GetChatHistory?recipientUserId=${recipientUserId}`)
        .then(response => response.json())
        .then(data => {
            messagesList.innerHTML = ""; // Clear the current messages

            data.forEach(message => {
                console.log("Message data:", message); // Debugging line

                var li = document.createElement("li");
                li.className = "list-group-item d-flex align-items-start";

                // Profile picture
                var img = document.createElement("img");
                img.src = message.profilePicture || "/images/default-profile.png"; // Fallback image if profilePicture is empty
                img.alt = `${message.senderNickName}'s profile picture`;

                // Message content container
                var messageContainer = document.createElement("div");

                var userSpan = document.createElement("strong");
                userSpan.textContent = `@${message.senderNickName}:`;

                var messageContent = document.createElement("p");
                messageContent.className = "message-content mb-0";
                messageContent.textContent = message.messageContent;

                var messageTimestamp = document.createElement("small");
                messageTimestamp.className = "message-timestamp";

                // Format timestamp
                var timestamp = message.timestamp;
                if (timestamp === undefined) {
                    console.error("Timestamp is undefined for message:", message);
                    messageTimestamp.textContent = "Unknown time"; // Fallback text
                } else {
                    var date = new Date(timestamp);
                    if (isNaN(date.getTime())) { // Check for invalid date
                        console.error("Invalid date:", timestamp);
                        messageTimestamp.textContent = "Invalid time"; // Fallback text
                    } else {
                        var options = { day: '2-digit', month: 'short', year: 'numeric', hour: '2-digit', minute: '2-digit', hour12: false };
                        messageTimestamp.textContent = date.toLocaleDateString('en-GB', options);
                    }
                }

                messageContainer.appendChild(userSpan);
                messageContainer.appendChild(messageContent);
                messageContainer.appendChild(messageTimestamp);

                li.appendChild(img);
                li.appendChild(messageContainer);

                messagesList.appendChild(li);
            });

            if (messagesList.children.length > 10) {
                messagesList.scrollTop = messagesList.scrollHeight;
            }
        })
        .catch(error => console.error('Error fetching chat history:', error));
});

function sendMessage() {
    var recipientUserId = document.getElementById("recipientUserId").value;
    var message = document.getElementById("messageInput").value;
    if (!recipientUserId || !message) {
        console.error("Recipient or message is empty.");
        return;
    }
    connection.invoke("SendMessage", parseInt(recipientUserId), message).catch(function (err) {
        return console.error(err.toString());
    });
    document.getElementById("messageInput").value = ""; // Clear the input field
}

