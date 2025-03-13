﻿




function GetAllUsers() {
    $.ajax({
        url: "/Home/GetAllUsers",
        method: "GET",
        success: function (data) {
            console.log(data);

            let content = "";
            for (var i = 0; i < data.length; i++) {
                let style = "";
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
                        <button class='btn btn-success' onclick="SendFollow('${data[i].id}')" >Follow</button>
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
                        <button class='btn btn-success'>Accept</button>
                        <button class='btn btn-warning'>Decline</button>
                    </div>
                    `;
                }
                else {
                    subContent = `
                    <div class='card-body'>
                        <button class='btn btn-warning'>Delete</button>
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

