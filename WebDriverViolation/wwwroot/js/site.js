var userId = document.getElementById("userId").value;
var connection = new signalR.HubConnectionBuilder().withUrl("/DriverViolation/NotificationHub?userId=" + userId).build();
connection.on("sendToUser", (Model) => {
   // alert(Model);
    var modelParsed = JSON.parse(Model);
    //alert(modelParsed['RealTimeViolationModel']['Id']);
    var notificationType = "";//modelParsed['RealTimeViolationModel']['ViolationType']['Name'];
    var count = document.getElementById("notificationCount1").innerText;
    document.getElementById("notificationCount1").innerText = parseInt(count) + 1;
    document.getElementById("notificationCount").innerText = parseInt(count) + 1 + " Notifications";
    var notificationList = document.getElementById("test");
    var dividerDiv = document.createElement("div");
    dividerDiv.setAttribute("class", "dropdown-divider");
    var a = document.createElement("a");
    a.removeAttribute("href");
    a.setAttribute("onclick", "showViolationDetails(" + JSON.stringify(modelParsed['RealTimeViolationModel']['Id']) + "," + JSON.stringify(modelParsed['ViolationTypeModels'] )+ "," + 0 + ")");
    a.setAttribute("class", "dropdown-item");
    var mediaDiv = document.createElement("div");
    mediaDiv.setAttribute("class", "media");
    var img = document.createElement("img");
    img.setAttribute("src", 'http://20.86.97.165/DriverViolation/images/alert.png');
    img.setAttribute("class", "img-size-50 img-circle mr-3");
    img.setAttribute("alt", "User Avatar");
    var mediaBodyDiv = document.createElement("div");
    mediaBodyDiv.setAttribute("class", "media-body");
    var h5 = document.createElement("h6");
    h5.innerText = "New Violation";
    var p = document.createElement("p");
    p.setAttribute("class", "text-sm");
    p.innerText = modelParsed['ViolationNotificationModel']['Message'];
    var dateP = document.createElement("p");
    dateP.setAttribute("class", "text-sm text-muted");
    dateP.innerText = modelParsed['ViolationNotificationModel']['CreatedDateText'] + "," + modelParsed['ViolationNotificationModel']['CreatedTimeText'];
    mediaBodyDiv.append(h5);
    mediaBodyDiv.append(p);
    mediaBodyDiv.append(dateP);
    mediaDiv.append(img);
    mediaDiv.append(mediaBodyDiv);
    a.append(mediaDiv);
    notificationList.append(dividerDiv);
    notificationList.prepend(a);
    var sound = document.getElementById("myAudio");
    sound.autoplay = true;
    sound.load();
    //var modelConverted = JSON.parse(modelParsed);
    addNewRow(modelParsed);

});

function addNewRow(modelConverted) {
   
    document.getElementById("pendingViolationCountId").textContent = modelConverted["PendingViolationCount"];
    var stratingTable = document.getElementById("pendingOrders");
    var row = stratingTable.insertRow(1);
    var rowId = "confirmButtonId" + modelConverted['RealTimeViolationModel']['Id'];
    row.setAttribute("id", rowId);
    var Number = row.insertCell(0);
    var Type = row.insertCell(1);
    var Probability = row.insertCell(2);
    var AvgProbability = row.insertCell(3);
    var Date = row.insertCell(4);
    Date.setAttribute('class', 'sparkbar');
    Date.setAttribute('data-color', '#00a65a');
    Date.setAttribute('data-height', '20');
    var Confirm = row.insertCell(5);
    Number.innerHTML = modelConverted['RealTimeViolationModel']['TruckID'];
    Type.innerHTML = modelConverted['RealTimeViolationModel']['ViolationType']['Name'];
    Date.innerHTML = modelConverted['RealTimeViolationModel']['Date'].toLocaleString('en-US');
    var a = document.createElement("a");
    a.setAttribute("onclick", "showViolationDetails(" + JSON.stringify(modelConverted['RealTimeViolationModel']['Id']) + ", " + JSON.stringify(modelConverted['ViolationTypeModels']) + "," + rowId + ")");
    a.setAttribute("class", "btn btn-primary");
    a.textContent = "Confirm";
    Confirm.append(a);
    Probability.innerHTML = modelConverted['RealTimeViolationModel']['Probability'];
    AvgProbability.innerHTML = modelConverted['RealTimeViolationModel']['AverageProbability'];
}

connection.start().catch(function (err) {
    return console.error(err.toString());
}).then(function () {
    connection.invoke('GetConnectionId').then(function (connectionId) {
    })
});
