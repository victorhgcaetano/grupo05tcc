using System;
using Microsoft.Xrm.Sdk;
using ConsoleIntegracao.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleIntegracao.Interface;
using Microsoft.Xrm.Sdk.Query;
using PluginIntegracao;

namespace ConsoleIntegracao.Model
{
    public class Contatos : IPadraoDeMetodos
    {
        public string TableName = "contact";
        public IOrganizationService Service { get; set; }

        public IOrganizationService connectDynamics2 = ConnectionFactory.GetCrmServiceDynamics2();

        public Contatos(IOrganizationService service)
        {
            this.Service = service;
        }

        public EntityCollection GetLista()
        {

            QueryExpression queryContats = new QueryExpression(this.TableName);
            queryContats.ColumnSet.AddColumns
               ("contactid",
                "firstname",
                "lastname",
                "fullname",
                "jrcv_cpf",
                "telephone1",
                "jobtitle",
                "parentcustomerid",
                "emailaddress1",
                "telephone1",
                "mobilephone",
                "fax",
                "preferredcontactmethodcode",
                "address1_line1",
                "address1_city",
                "address1_stateorprovince",
                "address1_postalcode",
                "address1_country"
                );

            return this.Service.RetrieveMultiple(queryContats);
        }

        public void CreateDataTable(EntityCollection dataTable)
        {
            Entity newContatc = new Entity(this.TableName);


            foreach (Entity contato in dataTable.Entities)
            {
                string contatoName = contato["firstname"].ToString();
                newContatc["firstname"] = contatoName;

                newContatc["contactid"] = contato["contactid"];

                string contatoSobrenome = contato.Contains("lastname") ? (contato["lastname"]).ToString() : string.Empty;
                newContatc["lastname"] = contatoSobrenome;

                string telephoneDoContato = contato.Contains("telephone1") ? (contato["telephone1"]).ToString() : string.Empty;
                newContatc["telephone1"] = telephoneDoContato;

                string cpf = contato.Contains("jrcv_cpf") ? (contato["jrcv_cpf"]).ToString() : string.Empty;
                newContatc["jrcv_cpf"] = cpf;

                string cargo = contato.Contains("jobtitle") ? (contato["jobtitle"]).ToString() : string.Empty;
                newContatc["jobtitle"] = cargo;

                EntityReference parentid = null;
                EntityReference verificada = VerificandoConta(contato);
                if (verificada != null)
                {
                    parentid = verificada;
                }

                newContatc["parentcustomerid"] = parentid;


                string email = contato.Contains("emailaddress1") ? (contato["emailaddress1"]).ToString() : string.Empty;
                newContatc["emailaddress1"] = email;

                string telephone = contato.Contains("telephone1") ? (contato["telephone1"]).ToString() : string.Empty;
                newContatc["telephone1"] = telephone;

                string celular = contato.Contains("mobilephone") ? (contato["mobilephone"]).ToString() : string.Empty;
                newContatc["mobilephone"] = celular;

                string fax = contato.Contains("fax") ? (contato["fax"]).ToString() : string.Empty;
                newContatc["fax"] = fax;

                OptionSetValue contatoPreferencial = contato.Contains("preferredcontactmethodcode") ? (OptionSetValue)contato["preferredcontactmethodcode"] : null;
                newContatc["preferredcontactmethodcode"] = contatoPreferencial;

                string endereco = contato.Contains("address1_line1") ? (contato["address1_line1"]).ToString() : string.Empty;
                newContatc["address1_line1"] = endereco;

                string cidade = contato.Contains("address1_city") ? (contato["address1_city"]).ToString() : string.Empty;
                newContatc["address1_city"] = cidade;

                string estado = contato.Contains("address1_stateorprovince") ? (contato["address1_stateorprovince"]).ToString() : string.Empty;
                newContatc["address1_stateorprovince"] = estado;

                string cep = contato.Contains("address1_postalcode") ? (contato["address1_postalcode"]).ToString() : string.Empty;
                newContatc["address1_postalcode"] = cep;

                string pais = contato.Contains("address1_country") ? (contato["address1_country"]).ToString() : string.Empty;
                newContatc["address1_country"] = pais;

                Service.Create(newContatc);
            }


        }

        public EntityReference VerificandoConta(Entity contato)
        {
            IOrganizationService d1 = ConnectionFactory.GetCrmServiceDynamics1();
            IOrganizationService d2 = ConnectionFactory.GetCrmServiceDynamics2();


            EntityReference contaId = contato.Contains("parentcustomerid") ? (EntityReference)contato["parentcustomerid"] : null;
            if (contaId == null)
            {
                return contaId;
            }

            Entity conta1 = d1.Retrieve("account", (Guid)contaId.Id, new ColumnSet("jrcv_cnpj", "name", "accountid"));


            string cnpj = conta1.Contains("jrcv_cnpj") ? conta1["jrcv_cnpj"].ToString() : string.Empty;

            if (cnpj != string.Empty)
            {
                QueryExpression queryConta2 = new QueryExpression("account");
                queryConta2.ColumnSet.AddColumns("name", "accountid", "jrcv_cnpj");
                queryConta2.Criteria.AddCondition("jrcv_cnpj", ConditionOperator.Equal, cnpj);

                EntityCollection contas = d2.RetrieveMultiple(queryConta2);

                if (contas != null)
                {
                    foreach (Entity account in contas.Entities)
                    {
                        Guid idContaDynamics2 = (Guid)account["accountid"];

                        contaId.Id = idContaDynamics2;

                    }
                }

            }
            else
            {
                contaId = null;
            }


            return contaId;
        }
    }
}