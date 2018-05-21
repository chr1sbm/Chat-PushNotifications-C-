$(document).ready(function () {
    $('#chatControl').hide();
});

var message = { username: ' ', text: ' ', dt: ' ' };

function setUser() {
    var username = document.getElementById("username").value;
    if (username == "" || username == undefined) {
        alert('Enter username');
        return;
    }
    else {

        message.username = username;
        $('#chatControl').show();
        $('#chatTemplate').empty();
        $('#start').hide();
        $('#message').text("Bienvenido: " + message.username).css("font-weight", "Bold");

    }


}
function Send() {
    
    message.text = document.getElementById("push").value;
    $.ajax({
        url: "http://localhost:52295/api/Chat/",
        data: JSON.stringify(message),
        cache: false,
        type: 'POST',
        dataType: "json",
        contentType: 'application/json; charset=utf-8'

    });
    $("#push").val('');

}

var source = new EventSource('http://localhost:52295/api/Chat/');

source.onmessage = function (e) {
    var data = e.data.split('|');
    var username = $("<strong></strong>").text(data[0] + " : ");
    var text = $("<i></i>").text(data[1]);
    var dt = $("<div></div>").text(data[2]);
    var chatTemp = document.createElement("p");
    chatTemp.append(dt[0], username[0], text[0], document.createElement("br"));
    $('#chatTemplate').append(chatTemp);
}
