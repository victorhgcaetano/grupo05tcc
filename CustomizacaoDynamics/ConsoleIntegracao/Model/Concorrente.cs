using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleIntegracao.Interface;

namespace ConsoleIntegracao.Model
{
    public class Concorrentes : IPadraoDeMetodos
    {
        public string TableName = "competitor";
        public IOrganizationService Service { get; set; }

        public Concorrentes(IOrganizationService service)
        {
            this.Service = service;
        }

        public EntityCollection GetLista()
        {
            QueryExpression queryAccount = new QueryExpression(this.TableName);
            queryAccount.ColumnSet.AddColumns
                ("name",
                "websiteurl",
                "transactioncurrencyid",
                "address1_line1",
                "address1_line2",
                "address1_line3",
                "address1_city",
                "address1_stateorprovince",
                "address1_postalcode",
                "address1_country"
                );

            return this.Service.RetrieveMultiple(queryAccount);
        }

        public void CreateDataTable(EntityCollection dataTable)
        {
            Entity competitor = new Entity(this.TableName);

            foreach (Entity concorrente in dataTable.Entities)
            {
                competitor["name"] = concorrente["name"].ToString();

                string site = concorrente.Contains("websiteurl") ? (concorrente["websiteurl"]).ToString() : string.Empty;
                competitor["websiteurl"] = site;

                EntityReference moeda = concorrente.Contains("transactioncurrencyid") ? (EntityReference)concorrente["transactioncurrencyid"] : null;
                competitor["transactioncurrencyid"] = moeda;

                string endereco = concorrente.Contains("address1_line1") ? (concorrente["address1_line1"]).ToString() : string.Empty;
                competitor["address1_line1"] = endereco;

                string endereco2 = concorrente.Contains("address1_line2") ? (concorrente["address1_line2"]).ToString() : string.Empty;
                competitor["address1_line2"] = endereco2;

                string endereco3 = concorrente.Contains("address1_line3") ? (concorrente["address1_line3"]).ToString() : string.Empty;
                competitor["address1_line3"] = endereco3;

                string cidade = concorrente.Contains("address1_city") ? (concorrente["address1_city"]).ToString() : string.Empty;
                competitor["address1_city"] = cidade;

                string cep = concorrente.Contains("address1_postalcode") ? (concorrente["address1_postalcode"]).ToString() : string.Empty;
                competitor["address1_postalcode"] = cep;

                string pais = concorrente.Contains("address1_country") ? (concorrente["address1_country"]).ToString() : string.Empty;
                competitor["address1_country"] = pais;

                Service.Create(competitor);
            }
        }
    }
}