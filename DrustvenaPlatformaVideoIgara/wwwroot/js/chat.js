"use strict";

var connection = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub")
    .configureLogging(signalR.LogLevel.Debug)
    .build();

connection.on("ReceiveMessage", function (user, message) {
    console.log(`Received message from ${user}: ${message}`); // Debugging line
    if (!user || !message) {
        console.error("Received invalid message:", { user, message });
        return;
    }

    var li = document.createElement("li");
    li.className = "list-group-item";
    li.textContent = `${user}: ${message}`;
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
    fetch(`/Users/GetChatHistory?recipientUserId=${recipientUserId}`)
        .then(response => response.json())
        .then(data => {
            var messagesList = document.getElementById("messagesList");
            messagesList.innerHTML = ""; // Clear the current messages

            data.forEach(message => {
                var li = document.createElement("li");
                li.className = "list-group-item";
                li.textContent = `${message.senderNickName} says ${message.messageContent}`;
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
    connection.invoke("SendMessage", parseInt(recipientUserId), message).catch(function (err) {
        return console.error(err.toString());
    });
    document.getElementById("messageInput").value = ""; // Clear the input field
}

console.log("messagesList element:", document.getElementById("messagesList"));
