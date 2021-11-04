using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleIntegracao.Model;
using Microsoft.Xrm.Sdk;
using PluginIntegracao;

namespace ConsoleIntegracao
{
    public class Program
    {
        static void Main(string[] args)
        {

            IOrganizationService connectDynamics1 = ConnectionFactory.GetCrmServiceDynamics1();
            IOrganizationService connectDynamics2 = ConnectionFactory.GetCrmServiceDynamics2();


            Contas getaccounts = new Contas(connectDynamics1);
            EntityCollection contasCrm = getaccounts.GetLista();

            Contas createAccounts = new Contas(connectDynamics2);
            createAccounts.CreateDataTable(contasCrm);


            Contatos getContacts = new Contatos(connectDynamics1);
            EntityCollection contatosCRM = getContacts.GetLista();

            Contatos createContacs = new Contatos(connectDynamics2);
            createContacs.CreateDataTable(contatosCRM);

            Concorrentes getCompetitor = new Concorrentes(connectDynamics1);
            EntityCollection concorrentesCRM = getCompetitor.GetLista();

            Concorrentes createCompetitor = new Concorrentes(connectDynamics2);
            createCompetitor.CreateDataTable(concorrentesCRM);

            Fatura getInvoice = new Fatura(connectDynamics1);
            EntityCollection faturasCRM = getInvoice.GetLista();

            Fatura createInvoice = new Fatura(connectDynamics2);
            createInvoice.CreateDataTable(faturasCRM);


            Console.WriteLine("Carga de Dados Finalizado!");
            Console.ReadLine();
        }

    }
}