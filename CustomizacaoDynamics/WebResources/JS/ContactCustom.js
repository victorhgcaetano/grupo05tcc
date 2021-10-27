if (typeof (Dynamics) == "undefined") { Dynamics = {} }
if (typeof (Dynamics.Contact) == "undefined") { Dynamics.Contact = {} }

Dynamics.Contact = {
    Attributes: {
        JRCV_CPF: "jrcv_cpf"
    },
    CPFOnChange: function (context) {
        debugger;
        var formContext = context.getFormContext();

        var cpf = formContext.getAttribute(Dynamics.Contact.Attributes.JRCV_CPF).getValue();
        
        if (cpf == "" || cpf == null)
            return;

        cpf = cpf.replace(".", "").replace(".", "").replace("-", "");

        if (cpf.length != 11) {
            formContext.getAttribute(Dynamics.Contact.Attributes.JRCV_CPF).setValue("");
            this.CustomAlert("Por favor, digite 11 caracteres no CPF", "Erro de Validação de CPF");
        }
        else {            
            if (this.CPFAuthenticate(cpf)) {
                cpf = cpf.replace(/^(\d{3})(\d{3})(\d{3})(\d{2})/, "$1.$2.$3-$4");
                formContext.getAttribute(Dynamics.Contact.Attributes.JRCV_CPF).setValue(cpf);
            } else {
                formContext.getAttribute(Dynamics.Contact.Attributes.JRCV_CPF).setValue("");
                this.CustomAlert("Por favor, digite um CPF valido", "Erro de Validação de CPF");
            }            
        }
    },
    CPFAuthenticate: function (cpf) {
        debugger;
        var primeiraValidacao = 0;
        var segundaValidacao = 0;        
        for (i = 0; i < cpf.length-1; i++) {            
            if (i < 9) {
                primeiraValidacao += cpf.charAt(i) * (10 - i);                
            }           
            segundaValidacao += cpf.charAt(i) * (11 - i);
        }
        primeiraValidacao = (((primeiraValidacao % 11) < 2) ? 0 : (11 - (primeiraValidacao % 11)));
        segundaValidacao = (((segundaValidacao % 11) < 2) ? 0 : (11 - (segundaValidacao % 11)));
        if ((primeiraValidacao == cpf.charAt(cpf.length - 2)) && (segundaValidacao == cpf.charAt(cpf.length - 1))) {
            return true;
        } else {
            return false;
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