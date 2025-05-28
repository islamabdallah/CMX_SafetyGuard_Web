'use strict';
function showViolationDetailslm(num, id, TypesList, confirmButtonId) {  
    var url = "../Violation/getViolationDetails";
    $.getJSON(url, { violationId: id }, function (data) {
        if (data != null) {
           // alert(data["Category"]);
            //console.log("welcome");
          //  console.log(data["Category"]);
            var count = (document.getElementById("notificationCount1").innerHTML);
            document.getElementById("notificationCount1").innerHTML = parseInt(count) - 1;
            //var count = (document.getElementById("notificationCount").innerHTML);
            document.getElementById("notificationCount").innerHTML = parseInt(count) - 1;
            if (num != 0) {
                document.getElementById("Name+" + num).hidden = true;
                document.getElementById("hrr+" + num).hidden = true;
            }
            
            document.getElementById("violationImgs").innerText = data["images"];
            //document.getElementById("lmCameraPositionns").selected = false;
            document.getElementById("currentViolationImgsNumber").innerText = 0;
            document.getElementById("AccuracyLevel").innerText = data["ViolationAccuracyLevelDescription"];
            document.getElementById("confirmedOrNot").innerText = confirmButtonId;
            document.getElementById("ViolationTruckID").innerText = data["TruckID"];
            document.getElementById("ViolationType").innerText = data["ViolationType"]["Name"];
            document.getElementById("ViolationDate").innerText = new Date(data["Date"]).toLocaleString('en-US');
            document.getElementById("ViolationImageID").src = data["images"][0];
            document.getElementById("ConfirmationStatusId").innerText = data["ConfirmationStatus"]["Name"];
            document.getElementById("SuggestedType").innerText = data["ViolationType"]["Name"];
            document.getElementById("Confidence").innerText = data["PriobabilityOfViolationsWithSameCode"][0];
            document.getElementById("violationProbabilities").innerText = data["PriobabilityOfViolationsWithSameCode"];
            document.getElementById("AverageConfidence").innerText = data["AverageProbability"];
            document.getElementById("violationId").textContent = data["Id"];
            if (data["ConfirmationStatus"]["Name"] == "Pending") {
                document.getElementById("ConfirmationBy").style.display = 'none';
                document.getElementById("NeedConfirmation").style.display = '';
                document.getElementById("RejectionButton").style.display = '';
                document.getElementById("RejectionText").style.display = 'none';
                var TypesDD = document.getElementById("ConfirmedTypeList");
                var movingstat = document.getElementById("CarMovingStatus");
                var lmtitle = document.getElementById("lmtitle");
                var lmtruck = document.getElementById("lmtruck");
                var lmmoving = document.getElementById("lmmoving");
                //movingstat.value = 'Stopped';

                if (data["IsTruckMoving"] == true) {
                    movingstat.value = 'Moving';
                }
                else {
                    movingstat.value = 'Stopped';
                }
                if (data["Category"] == "camera") {
                    //alert("done");
                    movingstat.hidden = true;
                    lmmoving.hidden = true;
                    lmtitle.innerText = "PPE Violations Details";
                    lmtruck.innerText = "Camera Name";
                }
                var positionss = document.getElementById("lmCameraPosition");
                positionss.value = '';
                var op1 = document.createElement('option');
                op1.text = 'Fit';
                op1.value = 'Fit';
                op1.selected = true;
                positionss.options[0] = op1;
                var op2 = document.createElement('option');
                op2.text = 'No-Fit';
                op2.value = 'No-Fit';
                op2.selected = false;
                positionss.options[1] = op2;

                if (TypesList.length > 0) {
                    for (var index = 0; index < TypesList.length; index++) {
                        var op = document.createElement('option');
                        op.text = TypesList[index].Name;
                        op.value = TypesList[index].ID;
                        if (TypesList[index].Name == data["ViolationType"]["Name"]) {
                            op.selected = true;
                        }
                        TypesDD.options[index] = op;
                    }
                }
            }
            else if (data["ConfirmationStatus"]["Name"] == "Confirmed") {
                document.getElementById("ConfirmationBy").style.display = '';
                document.getElementById("NeedConfirmation").style.display = 'none';
                document.getElementById("ConfirmedTypeId").innerText = data["ConfirmationViolationTypeName"];
                document.getElementById("ConfirmationById").innerText = data["ConfirmedByUserName"];
                document.getElementById("lmCameraPosition").innerText = data["CameraPosition"];
                document.getElementById("ConfirmedDate").innerText = data["ConfirmationDate"];
                document.getElementById("ConfirmationButton").style.display = 'none';
                if (data["ConfirmationViolationTypeName"] == "Rejected") {
                    document.getElementById("RejectionButton").style.display = 'none';
                    document.getElementById("ConfirmationButton").style.display = 'none';
                    document.getElementById("RejectionText").style.display = '';
                    document.getElementById("ConfirmedTypeId").style.color = 'red';
                }
                else {
                    document.getElementById("ConfirmedTypeId").style.color = 'green';
                    document.getElementById("RejectionText").style.display = '';
                    document.getElementById("RejectionText").innerText = "No Rejection";
                    document.getElementById("RejectionText").style.color = 'black';
                    document.getElementById("RejectionButton").style.display = 'none';
                    document.getElementById("ConfirmationButton").style.display = 'none';
                }
            }
            $('#ViolationNotificationModal').modal('show');
        }
    });
}


