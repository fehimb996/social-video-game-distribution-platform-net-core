"use strict";

// Initialize SignalR connection
var connection = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub")
    .configureLogging(signalR.LogLevel.Debug)
    .build();

// Function to scroll the messages list to the bottom
function scrollToBottom() {
    var messagesList = document.getElementById("messagesList");
    messagesList.scrollTop = messagesList.scrollHeight;
}

// Handle receiving a message
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
    userSpan.textContent = `${user}:`;

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

    // Scroll to the bottom when a new message is received
    scrollToBottom();
});

connection.start().then(function () {
    console.log("SignalR connection established."); // Debugging line
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    console.error("SignalR connection error:", err.toString()); // Debugging line
});

// Handle sending a message
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

// Handle recipient selection and load chat history
document.getElementById("friendsList").addEventListener("click", function (event) {
    var target = event.target.closest(".list-group-item");
    if (target) {
        var recipientUserId = target.getAttribute("data-user-id");
        var messagesList = document.getElementById("messagesList");

        // Highlight the selected friend
        document.querySelectorAll(".list-group-item").forEach(item => item.classList.remove("selected"));
        target.classList.add("selected");

        if (recipientUserId === "") {
            messagesList.innerHTML = "";
            return;
        }

        fetch(`/Users/GetChatHistory?recipientUserId=${recipientUserId}`)
            .then(response => response.json())
            .then(data => {
                messagesList.innerHTML = "";

                data.forEach(message => {
                    var li = document.createElement("li");
                    li.className = "list-group-item d-flex align-items-start";

                    var img = document.createElement("img");
                    img.src = message.profilePicture || "/images/default-profile.png";
                    img.alt = `${message.senderNickName}'s profile picture`;

                    var messageContainer = document.createElement("div");

                    var userSpan = document.createElement("strong");
                    userSpan.textContent = `${message.senderNickName}:`;

                    var messageContent = document.createElement("p");
                    messageContent.className = "message-content mb-0";
                    messageContent.textContent = message.messageContent;

                    var messageTimestamp = document.createElement("small");
                    messageTimestamp.className = "message-timestamp";
                    messageTimestamp.textContent = message.timestamp;

                    messageContainer.appendChild(userSpan);
                    messageContainer.appendChild(messageContent);
                    messageContainer.appendChild(messageTimestamp);

                    li.appendChild(img);
                    li.appendChild(messageContainer);

                    messagesList.appendChild(li);
                });

                // Scroll to the bottom after loading the chat history
                scrollToBottom();
            })
            .catch(error => console.error('Error fetching chat history:', error));
    }
});

// Function to send a message
function sendMessage() {
    var recipientUserId = document.querySelector(".list-group-item.selected")?.getAttribute("data-user-id");
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
