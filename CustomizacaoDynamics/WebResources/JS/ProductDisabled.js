if (typeof (Dynamics) == "undefined") { Dynamics = {} }
if (typeof (Dynamics.Product) == "undefined") { Dynamics.Product = {} }

Dynamics.Product = {
    OnLoad: function (context) {
        debugger;        
        var formContext = context.getFormContext();
        var tamanhoArray = formContext.getControl().length;
        for (i = 0; i < tamanhoArray; i++){
            formContext.getControl(i).setDisabled(true);
        }
        formContext.ui.headerSection.setCommandBarVisible(false)
    }, 
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
}