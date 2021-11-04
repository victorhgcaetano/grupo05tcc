if (typeof (Dynamics) == "undefined") { Dynamics = {} }
if (typeof (Dynamics.Opportunity) == "undefined") { Dynamics.Opportunity = {} }

Dynamics.Opportunity = {
    Attributes: {
        JRCV_CRIADOVIAPLUGIN: "jrcv_criadoviaplugin"
    },
    OnLoad: function (context) {
        debugger;
        var formContext = context.getFormContext();
        if (formContext.getAttribute(Dynamics.Opportunity.Attributes.JRCV_CRIADOVIAPLUGIN).getValue() == "SIM") {
            var tamanhoArray = formContext.getControl().length;
            for (i = 0; i < tamanhoArray; i++) {
                formContext.getControl(i).setDisabled(true);
            }
        }
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