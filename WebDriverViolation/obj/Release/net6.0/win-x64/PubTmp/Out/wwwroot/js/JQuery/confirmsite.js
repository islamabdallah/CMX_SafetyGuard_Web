'use strict';
function addNewConfirmation() {
    console.log("start");
    var rr = document.getElementById("lmCameraPosition").value;
    //  alert(rr);
    var Confirmdata =
    {
        cameraPosition: document.getElementById("lmCameraPosition").value,
        selectedTypeId: document.getElementById("ConfirmedTypeList").value,
        Id: document.getElementById("violationId").textContent,
    };
    $.ajax(
        {
            // url: '@Url.Content("~/")' + "Violation/AddConfirmationData",
            url: "../Violation/AddConfirmationData",
            data: Confirmdata,
            type: "POST",
            async: false,
            success: function (data) {
                swal("Success Process",
                    {
                        icon: "success",
                    });
                var url = "../Home/getViolationStats";
                //var url = '@Url.Content("~/")' + "Home/getViolationStats";
                $.getJSON(url, function (data) {
                    if (data != null) {

                        // document.getElementById("pendingViolationCountId").innerText = data['PendingViolationCount'];
                        // console.log(document.getElementById("pendingViolationCountId").innerText);
                        // consol.log(data['PendingViolationCount'])
                        // cons.log("step1")
                        // document.getElementById("thisMonthActualCountId").innerText = data['CurrentMonthActualViolationCount'];
                        // console.log(document.getElementById("thisMonthActualCountId").innerText);
                        // consol.log(data['CurrentMonthActualViolationCount'])
                        // cons.log("step2")
                        // document.getElementById("totalActualCountId").innerText = data['TotalActualViolationCount'];
                        // console.log(document.getElementById("totalActualCountId").innerText);
                        // consol.log(data['TotalActualViolationCount'])
                        // cons.log("step3")
                        // for (var index = 0; index < data["ViolationTypeCounts"].length; index++) {
                        //     document.getElementById(data['ViolationTypeCounts'][index]["ViolationTypeName"]).innerText = data['ViolationTypeCounts'][index]["Count"];
                        // }
                    }
                });
                var buttonId = document.getElementById("confirmedOrNot").innerText;
                var x = document.getElementById("rowIndex").innerText;
                //alert("index");
                // alert(x);
                if (buttonId != 0) {
                    if (x == -1) {
                        // alert("don");
                        // cons.log(-1);
                        document.getElementById(buttonId).cells[3].innerHTML = "<b style='color:green'>Confirmed</b>";
                        if (rr == 10) {
                            // alert(rr);
                            //  console.log(rr);
                            // console.log(-2);
                            document.getElementById(buttonId).cells[5].innerHTML = "<b style='color:green'>No Violation</b>";
                        }
                        else {
                            // console.log(-3);
                            document.getElementById(buttonId).cells[5].innerHTML = "<b style='color:red'>Violation</b>";
                        }
                    }
                    else {
                        // alert("No");
                        // alert(buttonId);
                        console.log(buttonId);
                        // console.log(-4);
                        document.getElementById(buttonId).remove();

                        // document.getElementById("movetable").deleteRow(parseInt(x) - 1);
                    }

                }
                document.getElementById("RejectionText").innerText = "No Rejection";
            },
            error: function () {
                swal("Failed process",
                    {
                        icon: "error",
                    });
            }

        });
    $('#ViolationNotificationModal').modal('toggle');
}