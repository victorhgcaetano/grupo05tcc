if (typeof (Dynamics) == "undefined") { Dynamics = {} }
if (typeof (Dynamics.Account) == "undefined") { Dynamics.Account = {} }

Dynamics.Account = {
    Attributes: {
        JRCV_CNPJ: "jrcv_cnpj",
        Name: "name",
        CEP1: "address1_postalcode",
        CEP2: "address2_postalcode",
        JRCV_Porte: "jrcv_porte",
        JRCV_NivelDoCliente: "jrcv_niveldocliente"
    },
    JRCV_Porte: {
        Pequeno: 100000000,                 
        Médio: 100000001,               
        Grande: 100000002
    },
    JRCV_NivelDoCliente: {
        Silver: 100000000,
        Gold: 100000001,
        Platinum: 100000002
    },
    OnLoad: function (context) {
        debugger;
        var formContext = context.getFormContext();
        formContext.getControl(Dynamics.Account.Attributes.JRCV_NivelDoCliente).setDisabled(true);      
    },
    CNPJOnChange: function (context) {
        debugger;
        var formContext = context.getFormContext();

        var cnpj = formContext.getAttribute(Dynamics.Account.Attributes.JRCV_CNPJ).getValue();

        if (cnpj == "" || cnpj == null)
            return;

        cnpj = cnpj.replace(".", "").replace(".", "").replace("/", "").replace("-", "");

        if (cnpj.length != 14) {
            formContext.getAttribute(Dynamics.Account.Attributes.JRCV_CNPJ).setValue("");
            this.CustomAlert("Por favor, digite 14 caracteres no CNPJ", "Erro de Validação de CNPJ");
        }
        else {            
            if (this.CNPJAuthenticate(cnpj)) {
                cnpj = cnpj.replace(/^(\d{2})(\d{3})(\d{3})(\d{4})(\d{2})/, "$1.$2.$3/$4-$5");
                formContext.getAttribute(Dynamics.Account.Attributes.JRCV_CNPJ).setValue(cnpj);                
            } else {
                formContext.getAttribute(Dynamics.Account.Attributes.JRCV_CNPJ).setValue("");
                this.this.CustomAlert("Por favor, digite um CNPJ valido", "Erro de Validação de CNPJ");
            }   
        }
    },
    CNPJAuthenticate: function (cnpj) {        
        var valida = new Array(6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2);        
        var digitoVerificador = (1 * cnpj.charAt(12)) + cnpj.charAt(13);
        var primeiraValidacao = 0;
        var segundaValidacao = 0;
        for (i = 0; i < valida.length; i++) {
            primeiraValidacao += (i > 0 ? (cnpj.charAt(i - 1) * valida[i]) : 0);
            segundaValidacao += cnpj.charAt(i) * valida[i];
        }
        primeiraValidacao = (((primeiraValidacao % 11) < 2) ? 0 : (11 - (primeiraValidacao % 11)));
        segundaValidacao = (((segundaValidacao % 11) < 2) ? 0 : (11 - (segundaValidacao % 11)));

        if (((primeiraValidacao * 10) + segundaValidacao) == digitoVerificador) {
            return true;
        } else {
            return false;
        }
    },
    CEPOnChange: function (context) {
        debugger;
        this.CEPMask(context, Dynamics.Account.Attributes.CEP1);
        //this.CEPOnChange(context, Dynamics.Account.Attributes.CEP2);
    },
    CEPMask: function (context, cepField) {
        debugger;
        var formContext = context.getFormContext();
        var cep = formContext.getAttribute(cepField).getValue();

        if (cep == "" || cep == null)
            return;

        cep = cep.replace(".", "").replace("-", "");

        if (cep.length != 8) {
            formContext.getAttribute(cepField).setValue("");
            this.CustomAlert("Por favor, digite 8 caracteres no CEP", "Erro de Validação de CEP");
        }
        else {            
            cep = cep.replace(/^(\d{2})(\d{3})(\d{3})/, "$1$2-$3");
            formContext.getAttribute(cepField).setValue(cep);
        }
    },
    NomeOnChange: function (context) {
        var formContext = context.getFormContext();
        var name = formContext.getAttribute(Dynamics.Account.Attributes.Name).getValue();

        if (name == "" || name == null)
            return;

        name = name.toLowerCase();
        var words = name.split(" ");
        for (i = 0; i < words.length; i++) {
            if (i == 0 || words[i].length > 2) {
                words[i] = words[i][0].toUpperCase() + words[i].slice(1);
            }
        }
        name = words.join(" ");
        formContext.getAttribute(Dynamics.Account.Attributes.Name).setValue(name);
    },
    PorteOnChange: function (context) {
        debugger;
        var formContext = context.getFormContext();
        var porte = formContext.getAttribute(Dynamics.Account.Attributes.JRCV_Porte).getValue();
        switch (porte) {
            case Dynamics.Account.JRCV_Porte.Pequeno:
                formContext.getAttribute(Dynamics.Account.Attributes.JRCV_NivelDoCliente).setValue(Dynamics.Account.JRCV_NivelDoCliente.Silver);
                break;
            case Dynamics.Account.JRCV_Porte.Médio:
                formContext.getAttribute(Dynamics.Account.Attributes.JRCV_NivelDoCliente).setValue(Dynamics.Account.JRCV_NivelDoCliente.Gold);
                break;
            default:
                formContext.getAttribute(Dynamics.Account.Attributes.JRCV_NivelDoCliente).setValue(Dynamics.Account.JRCV_NivelDoCliente.Platinum);
                break;
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