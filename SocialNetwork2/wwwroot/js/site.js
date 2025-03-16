




function GetAllUsers() {
    $.ajax({
        url: "/Home/GetAllUsers",
        method: "GET",
        success: function (data) {

            let content = "";
            for (var i = 0; i < data.length; i++) {
                let style = "";
                let subContent = "";
                if (data[i].hasRequestPending) {
                    subContent = `<button class='btn btn-outline-secondary' onclick="TakeRequest('${data[i].id}')" >Already Sent</button>`;
                }
                else {
                    if (data[i].isFriend) {
                        subContent = ` <button class='btn btn-secondary' onclick="UnFollow('${data[i].id}')" >UnFollow</button>
                        <a class='btn btn-outline-success' href='Home/GoChat/${data[i].id}'>Go Chat</a>
                        `;
                    }
                    else {
                        subContent = ` <button class='btn btn-success' onclick="SendFollow('${data[i].id}')" >Follow</button>`;
                    }
                }
                if (data[i].isOnline) {
                    style = "border:5px solid springgreen";
                }
                else {
                    style = "border:5px solid red";
                }

                const item = `
                    <div class='card' style='${style};width:300px;margin:5px;'>
                        <img style='width:100%;height:200px' src='/images/${data[i].image}' />
                        <div class='card-body'>
                        <h5 class='card-title'>${data[i].userName}</h5>
                        <p> ${data[i].email} </p>
                            ${subContent}
                        </div>
                    </div>
                `;
                content += item;
            }

            $("#allUsers").html(content);
        }
    });
}

function GetMyRequests() {
    $.ajax({
        url: `/Home/GetAllRequests`,
        method: "GET",
        success: function (data) {
            $("#requests").html("");
            let content = "";
            let subContent = "";

            for (var i = 0; i < data.length; i++) {
                if (data[i].status == "Request") {
                    subContent = `
                    <div class='card-body'>
                        <button class='btn btn-success' onclick="AcceptRequest('${data[i].senderId}','${data[i].receiverId}',${data[i].id})">Accept</button>
                        <button class='btn btn-warning' onclick="DeclineRequest(${data[i].id},'${data[i].senderId}')">Decline</button>
                    </div>
                    `;
                }
                else {
                    subContent = `
                    <div class='card-body'>
                        <button class='btn btn-warning' onclick="DeleteRequest(${data[i].id})">Delete</button>
                    </div>
                    `;
                }
                let item = `
                <div class='card' style='width:15rem;'>
                <div class='card-body'>
                   <h5>Request</h5>
                   <ul class='list-group list-group-flush'>
                   <li>${data[i].content}</li>
                   ${subContent}
                   </ul>
                </div>
                </div>
                `;
                content += item;
            }
            $("#requests").html(content);
        }
    })
}
GetMyRequests();

GetAllUsers();

function UnFollow(receiverId) {
    $.ajax({
        url: `/Home/UnFollow?receiverId=${receiverId}`,
        method: 'DELETE',
        success: function (data) {
            GetAllUsers();
            GetMyRequests();
            SendFollowCall(receiverId);
        }
    })
}

function GetSoundOfMessage() {
    //var audio = new Audio('~/js/bip.mp3');
    //audio.play();
}

function SendMessage(receiverId, senderId) {
    const content = document.querySelector("#message-input");
    let obj = {
        receiverId: receiverId,
        senderId: senderId,
        content: content.value
    };

    $.ajax({
        url: `/Home/AddMessage`,
        method: "POST",
        data:obj,
        success: function (data) {
            GetMessageCall(receiverId, senderId);
            content.value = "";
        }
    })
}

function GetMessages(receiverId, senderId) {
    console.log("receiverId", receiverId);
    console.log("senderUd", senderId);
   
    $.ajax({
        url: `/Home/GetAllMessages?receiverId=${receiverId}&senderId=${senderId}`,
        method: "GET",
        success: function (data) {
            let content = "";
            for (var i = 0; i < data.messages.length; i++) {
                let item = ``;
                if (data.currentUserId == data.messages[i].senderId) {
                    item = ` 
                    <section style="display:flex;margin-top:25px;border:4px solid springgreen;
margin-left:150px;border-radius:20px 0 0 20px;width:50%;padding:20px;
">
                        <h5>${data.messages[i].content}</h5>
                            <p>${data.messages[i].dateTime}</p>
                        </section>
                    `;
                }
                else {
                    item = ` 
                    <section style="display:flex;margin-top:25px;border:4px solid deepskyblue;
margin-left:0;border-radius:0 20px 20px 0;width:50%;padding:20px;
">
                        <h5>${data.messages[i].content}</h5>
                            <p>${data.messages[i].dateTime}</p>
                        </section>
                    `;
                }
                content += item;
            }

            $("#currentMessages").html(content);
            var objDiv = document.getElementById("currentMessages");
            objDiv.scrollTop = objDiv.scrollHeight;
        }
    })


}


function DeleteRequest(id) {
    $.ajax({
        url: `/Home/DeleteRequest/${id}`,
        method: 'DELETE',
        success: function (data) {
            GetMyRequests();
        }
    })
}

function AcceptRequest(senderId, receiverId, id) {
    $.ajax({
        url: `/Home/AcceptRequest?senderId=${senderId}&receiverId=${receiverId}&requestId=${id}`,
        method: "GET",
        success: function (data) {
            const element = document.querySelector("#alert");
            element.style.display = "block";
            element.innerHTML = "You accept request successfully";

            SendFollowCall(senderId);
            SendFollowCall(receiverId);

            setTimeout(() => {
                element.innerHTML = "";
                element.style.display = "none";
            }, 5000);
        }
    })
}

function SendFollow(id) {
    const element = document.querySelector("#alert");
    element.style.display = "none";
    $.ajax({
        url: `/Home/SendFollow/${id}`,
        method: "GET",
        success: function (data) {
            element.style.display = "block";
            element.innerHTML = "Your friend request sent successfully";
            SendFollowCall(id);
            GetAllUsers();
            setTimeout(() => {
                element.innerHTML = "";
                element.style.display = "none";
            }, 5000);
        }
    })
}

function DeclineRequest(id, senderId) {
    $.ajax({
        url: `/Home/DeclineRequest?id=${id}&senderId=${senderId}`,
        method: "GET",
        success: function (data) {
            const element = document.querySelector("#alert");
            element.style.display = "block";
            element.innerHTML = "You declined request";

            SendFollowCall(senderId);
            GetMyRequests();
            GetAllUsers();
            setTimeout(() => {
                element.innerHTML = "";
                element.style.display = "none";
            }, 5000)
        }
    })
}

function TakeRequest(receiverId) {
    $.ajax({
        url: `/Home/TakeRequest?id=${receiverId}`,
        method: "DELETE",
        success: function (data) {
            const element = document.querySelector("#alert");
            element.style.display = "block";
            element.innerHTML = "You has taken your request successfully";

            SendFollowCall(receiverId);
            GetMyRequests();
            GetAllUsers();
            setTimeout(() => {
                element.innerHTML = "";
                element.style.display = "none";
            }, 5000)
        }
    })
}