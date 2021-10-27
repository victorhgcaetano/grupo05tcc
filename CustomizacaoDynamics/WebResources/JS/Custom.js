CustomAlert: function (alertText, alertTitle) {
    var alertStrings = {
        confirmButtonLabel: "OK",
        text: alertText,
        title: alertTitle
    };

    var alertOptions = {
        heigth: 120,
        width: 200
    };

    Xrm.Navigation.openAlertDialog(alertStrings, alertOptions);
}