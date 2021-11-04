using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginIntegracao.Notification
{
    public class WebServices
    {
        public void CreateAccount(IOrganizationService service, string Name)
        {
            Entity notification = new Entity("jrcv_notificacaodocliente");
            notification["jrcv_name"] = Name;
            notification["jrcv_mensagem"] = "Obrigado por contratar nossos serviços!";
            notification["jrcv_datadanotificacao"] = DateTime.Now;

            service.Create(notification);
        }
    }
}