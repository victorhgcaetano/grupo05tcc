using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using ConsoleIntegracao.Interface;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PluginIntegracao;

namespace ConsoleIntegracao.Model
{
   public class Fatura : IPadraoDeMetodos
    {
        public string TableName = "invoice";
        public IOrganizationService Service { get; set; }

        public IOrganizationService connectDynamics2 = ConnectionFactory.GetCrmServiceDynamics2();

        public Fatura(IOrganizationService service)
        {
            this.Service = service;
        }
        public EntityCollection GetLista()
        {
            QueryExpression queryAccount = new QueryExpression(this.TableName);
            queryAccount.ColumnSet.AddColumns
                ("invoicenumber",
                "name",
                "transactioncurrencyid",
                "ispricelocked",
                "datedelivered",
                "duedate",
                "shippingmethodcode",
                "paymenttermscode",
                "totallineitemamount",
                "discountpercentage",
                "discountamount",
                "opportunityid",
                "salesorderid",
                "customerid"
                );

            return this.Service.RetrieveMultiple(queryAccount);
        }

        public void CreateDataTable(EntityCollection dataTable)
        {

            Entity invoice = new Entity(this.TableName);

            Contatos getContacts = new Contatos(connectDynamics2);
            EntityCollection contatos = getContacts.GetLista();

            foreach (Entity fatura in dataTable.Entities)
            {
                invoice["invoicenumber"] = fatura["invoicenumber"].ToString();

                invoice["name"] = fatura["name"].ToString();

                EntityReference moeda = fatura.Contains("transactioncurrencyid") ? (EntityReference)fatura["transactioncurrencyid"] : null;
                invoice["transactioncurrencyid"] = moeda;


                invoice["ispricelocked"] = fatura["ispricelocked"];

                DateTime? nullDate = null;
                DateTime? dataEntrega = fatura.Contains("datedelivered") ? ((DateTime)fatura["datedelivered"]) : nullDate;
                invoice["datedelivered"] = dataEntrega;

                DateTime? dataConclusao = fatura.Contains("duedate") ? ((DateTime)fatura["duedate"]) : nullDate;
                invoice["duedate"] = dataConclusao;

                OptionSetValue formaTransporte = fatura.Contains("shippingmethodcode") ? (OptionSetValue)fatura["shippingmethodcode"] : null;
                invoice["shippingmethodcode"] = formaTransporte;

                OptionSetValue condicaoPagamento = fatura.Contains("paymenttermscode") ? (OptionSetValue)fatura["paymenttermscode"] : null;
                invoice["paymenttermscode"] = condicaoPagamento;

                var valorDetalhado = fatura.Contains("totallineitemamount") ? (fatura["totallineitemamount"]) : null;
                invoice["totallineitemamount"] = valorDetalhado;

                Decimal descontoFatura = fatura.Contains("discountpercentage") ? (Decimal)(fatura["discountpercentage"]) : 0;
                invoice["discountpercentage"] = descontoFatura;

                Money valorDescontonaFatura = fatura.Contains("discountamount") ? (Money)((AliasedValue)fatura["discountamount"]).Value : null;
                invoice["discountamount"] = valorDescontonaFatura;

                EntityReference opportunity = fatura.Contains("opportunityid") ? (EntityReference)fatura["opportunityid"] : null;
                invoice["opportunityid"] = opportunity;

                EntityReference contrato = fatura.Contains("salesorderid") ? (EntityReference)fatura["salesorderid"] : null;
                invoice["salesorderid"] = contrato;

                EntityReference client = fatura.Contains("customerid") ? (EntityReference)fatura["customerid"] : null;
                if (client != null)
                {
                    foreach (Entity contact in contatos.Entities)
                    {
                        string contactName = contact["fullname"].ToString();
                        if (client.Name == contactName)
                        {
                            string idContato = contact["contactid"].ToString();
                            Guid id = new Guid(idContato);
                            client.Id = id;
                            invoice["customerid"] = client;
                            break;
                        }
                        else
                        { invoice["customerid"] = null; }

                    }
                }
                else
                {
                    invoice["customerid"] = client;
                }

                Service.Create(invoice);
            }
        }

    }
}