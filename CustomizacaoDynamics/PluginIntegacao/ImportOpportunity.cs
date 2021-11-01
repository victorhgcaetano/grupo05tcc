﻿using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginIntegracao
{
    public class ImportOpportunity : PluginImplementation
    {
        public string TableName = "opportunity";
        public override void ExecutePlugin(IServiceProvider serviceProvider)
        {
            if (Context.MessageName == "Create")
            {
                CreateUpdateOpportunity();                
            }
            else if (Context.MessageName == "Update")
            {
                CreateUpdateOpportunity();
            }
            else if (Context.MessageName == "Delete")
            {
                DeleteOpportunity();
            }
            //throw new InvalidPluginExecutionException("Passei primeiro no ImportOpp");
        }

        private void DeleteOpportunity()
        {
            //FUNÇÃO PARA DELETAR OPORTUNIDADES NO DYNAMICS2

            Entity preDeleteImage = Context.PreEntityImages["PreImage"];
            string opportunityName = preDeleteImage["name"].ToString();
            //PEGAR NOME DA OPORTUNIDADE PELA PRE-IMAGE

            IOrganizationService service = ConnectionFactory.GetCrmServiceDynamics2();

            EntityCollection opportunityId = RetrieveOpportunity(opportunityName);
            //CHAMAR FUNÇÃO PARA PEGAR O ID DA OPORTUNIDADE DO DYNAMICS1 

            foreach (Entity opportunity in opportunityId.Entities)
            {
                Guid OpportunityId = (Guid)opportunity["opportunityid"];
                service.Delete(TableName, OpportunityId);
            }
        }

        private EntityCollection RetrieveOpportunity(string name)
        {
            //FUNÇÃO PARA FAZER RETRIEVE DO ID DA OPORTUNIDADE NO DYNAMICS2 POR MEIO DO NOME DA OPORTUNIDADE

            IOrganizationService service = ConnectionFactory.GetCrmServiceDynamics2();

            QueryExpression queryRetrieveOpportunity = new QueryExpression(TableName);
            queryRetrieveOpportunity.ColumnSet.AddColumns("opportunityid");
            queryRetrieveOpportunity.Criteria.AddCondition("name", ConditionOperator.Equal, name);


            return service.RetrieveMultiple(queryRetrieveOpportunity);
        }

        private void UpdateOpportunity()
        {
            //FUNÇÃO PARA ATUALIZAR AS OPORTUNIDADES
            Entity preOpportunityImage = Context.PreEntityImages["PreImage"];

            string opportunityName = preOpportunityImage.Contains("name") ? preOpportunityImage["name"].ToString() : string.Empty;
            EntityReference account = (EntityReference)preOpportunityImage["parentaccountid"];
            EntityReference contact = (EntityReference)preOpportunityImage["parentcontactid"];
            OptionSetValue tempoDeCompra = preOpportunityImage.Contains("purchasetimeframe") ?
                (OptionSetValue)preOpportunityImage["purchasetimeframe"] : null;
            string descricao = preOpportunityImage.Contains("description") ? preOpportunityImage["description"].ToString() : string.Empty;
            Money valorOrcamento = preOpportunityImage.Contains("budgetamount") ? (Money)preOpportunityImage["budgetamount"] : null;
            OptionSetValue previsao = preOpportunityImage.Contains("msdyn_forecastcategory") ? (OptionSetValue)preOpportunityImage["msdyn_forecastcategory"] : null;

            IOrganizationService service = ConnectionFactory.GetCrmServiceDynamics2();

            EntityCollection accountName = RetrieveAccountAndOpportunityName("account", account.Id, "parentaccountid", service);

            foreach (Entity accountsName in accountName.Entities)
            {

                string nomeDaConta = accountsName["name"].ToString();

                EntityCollection accountId = RetrieveId("account", "accountid", nomeDaConta, service);

                foreach (Entity accountsId in accountId.Entities)
                {
                    Guid idDaConta = (Guid)accountsId["accountid"];

                    EntityCollection contactName = RetrieveAccountAndOpportunityName("contact", contact.Id, "parentcontactid", service);

                    foreach (Entity contactsName in contactName.Entities)
                    {
                        string nomeDoContato = contactsName["name"].ToString();

                        EntityCollection contactId = RetrieveId("contact", "contactid", nomeDoContato, service);

                        foreach (Entity contactsId in contactId.Entities)
                        {
                            Guid idDoContato = (Guid)contactsId["contactid"];

                            EntityCollection opportunityId = RetrieveId("opportunity", "opportunityid", opportunityName, service);

                            foreach (Entity opportunitiesId in opportunityId.Entities)
                            {

                                Guid idDaOportunidade = (Guid)opportunitiesId["opportunityid"];
                                Entity createOpportunityInDynamics2 = new Entity(TableName);

                                createOpportunityInDynamics2.Id = idDaOportunidade;
                                createOpportunityInDynamics2["name"] = preOpportunityImage["name"];
                                createOpportunityInDynamics2["parentaccountid"] = new EntityReference("account", idDaConta);
                                createOpportunityInDynamics2["parentcontactid"] = new EntityReference("contact", idDoContato);
                                createOpportunityInDynamics2["purchasetimeframe"] = (OptionSetValue)tempoDeCompra;
                                createOpportunityInDynamics2["description"] = descricao;
                                createOpportunityInDynamics2["budgetamount"] = (Money)valorOrcamento;
                                createOpportunityInDynamics2["msdyn_forecastcategory"] = (OptionSetValue)previsao;

                                service.Update(createOpportunityInDynamics2);
                            }
                        }
                    }

                }
            }
        }

        private void CreateOpportunity()
        {
            
            //FUNÇÃO PARA CRIAR OPORTUNIDADES NO DYNAMICS2
            Entity opportunity = (Entity)this.Context.InputParameters["Target"];
            
            //string opportunityName = opportunity.Contains("name") ? opportunity["name"].ToString() : string.Empty;
            //EntityReference account = (EntityReference)opportunity["parentaccountid"];
            //EntityReference contact = (EntityReference)opportunity["parentcontactid"];
            //OptionSetValue tempoDeCompra = opportunity.Contains("purchasetimeframe") ?
            //    (OptionSetValue)opportunity["purchasetimeframe"] : null;
            //string descricao = opportunity.Contains("description") ? opportunity["description"].ToString() : string.Empty;
            //Money valorOrcamento = opportunity.Contains("budgetamount") ? (Money)opportunity["budgetamount"] : null;
            //OptionSetValue previsao = opportunity.Contains("msdyn_forecastcategory") ? (OptionSetValue)opportunity["msdyn_forecastcategory"] : null;
            //
            //IOrganizationService service = ConnectionFactory.GetCrmServiceDynamics2();
            //string[] accountColumns = { "accountid", "name", "jrcv_cnpj", "jrcv_porte", "jrcv_niveldocliente" };
            //EntityCollection accountName = RetrieveAccountAndOpportunityName("account", (Guid)account.Id, "accountid", service, accountColumns);

            //foreach (Entity accountsName in accountName.Entities)
            //{
            //
            //    string nomeDaConta = accountsName["name"].ToString();
            //
            //    EntityCollection accountId = RetrieveId("account", "accountid", nomeDaConta, service);
            //
            //    foreach (Entity accountsId in accountId.Entities)
            //    {
            //        Guid idDaConta = (Guid)accountsId["accountid"];
            //        string[] contactColumns = { "contactid", "fullname", "jrcv_cpf" };
            //        EntityCollection contactName = RetrieveAccountAndOpportunityName("contact", (Guid)contact.Id, "contactid", service, contactColumns);
            //
            //        foreach (Entity contactsName in contactName.Entities)
            //        {
            //            string nomeDoContato = contactsName["name"].ToString();
            //
            //            EntityCollection contactId = RetrieveId("contact", "contactid", nomeDoContato, service);
            //
            //            foreach (Entity contactsId in contactId.Entities)
            //            {
            //                Guid idDoContato = (Guid)contactsId["contactid"];
            //
            //                Entity createOpportunityInDynamics2 = new Entity(TableName);
            //
             //           Entity createOpportunityInDynamics2 = new Entity();
             //               createOpportunityInDynamics2["name"] = opportunity["name"];
             //               //createOpportunityInDynamics2["parentaccountid"] = new EntityReference("account", idDaConta);
             //               //createOpportunityInDynamics2["parentcontactid"] = new EntityReference("contact", idDoContato);
             //               createOpportunityInDynamics2["purchasetimeframe"] = (OptionSetValue)tempoDeCompra;
             //               createOpportunityInDynamics2["description"] = descricao;
             //               createOpportunityInDynamics2["budgetamount"] = (Money)valorOrcamento;
             //               createOpportunityInDynamics2["msdyn_forecastcategory"] = (OptionSetValue)previsao;
             //               createOpportunityInDynamics2["jrcv_criadoviaplugin"] = "SIM";
             //
             //               service.Create(createOpportunityInDynamics2);
            //            }
            //        }
            //
            //    }
            //}
            
        
            IOrganizationService service = ConnectionFactory.GetCrmServiceDynamics2();
            Entity opportunityInDynamics2 = opportunity;

            //opportunityInDynamics2["jrcv_origemdoregistro"] = "Dynamics1";
             opportunityInDynamics2["jrcv_criadoviaplugin"] = "SIM";
            //opportunity["jrcv_criadoviaplugin"] = "SIM";
            service.Create(opportunityInDynamics2);
            ////service.Create(GetReferences(opportunity,service));
            ///
            
        }

        private void CreateUpdateOpportunity()
        {
            //FUNÇÃO PARA CRIAR OPORTUNIDADES NO DYNAMICS2
            Entity opportunity = (Entity)this.Context.InputParameters["Target"];            
            IOrganizationService service = ConnectionFactory.GetCrmServiceDynamics2();
            //Entity opportunityInDynamics2 = opportunity;                             
            //opportunity["jrcv_criadoviaplugin"] = "SIM";
            //service.Create(opportunityInDynamics2);
            service.Create(GetReferences(opportunity,service));            
        }

        private Entity GetReferences(Entity opportunity, IOrganizationService service)
        {
            Entity opportunityInDynamics2 = opportunity;
            EntityReference account = (EntityReference)opportunity["parentaccountid"];
            EntityReference contact = (EntityReference)opportunity["parentcontactid"];
            opportunityInDynamics2["jrcv_criadoviaplugin"] = "SIM"; // identificador de origem do registro

            // PESQUISAR SE NO DESTINO TEM A CONTA
            string[] accountColumns = { "accountid", "name", "jrcv_cnpj", "jrcv_porte", "jrcv_niveldocliente" };
            EntityCollection accountName = RetrieveAccountAndOpportunityName("account", (Guid) account.Id, "accountid", service, accountColumns);
            if (accountName.Entities.Count() == 0)
            {
                // SE NÃO TEM, PEGA A CONTA DA ORIGEM E CRIA NO DESTINO
                accountName = RetrieveAccountAndOpportunityName("account", (Guid) account.Id, "accountid", this.Service, accountColumns);
                service.Create(accountName[0]);
            }
            else
            {
                // SE EXISTE NO DESTINO, ALTERA O ID DA CONTA PARA O DO DESTINO
                opportunityInDynamics2["parentaccountid"] = new EntityReference("account", (Guid) accountName[0].Id);
            }

            string[] contactColumns = { "contactid", "fullname", "jrcv_cpf" };
            EntityCollection contactId = RetrieveAccountAndOpportunityName("contact", contact.Id, "contactid", service, contactColumns);
            if (contactId.Entities.Count() == 0)
            {
                contactId = RetrieveAccountAndOpportunityName("contact", contact.Id, "contactid", this.Service, contactColumns);
                service.Create(contactId[0]);
            }
            else
            {
                opportunityInDynamics2["parentcontactid"] = new EntityReference("contact", (Guid) contactId[0].Id);

            }

            return opportunityInDynamics2;
        }

        private EntityCollection RetrieveAccountAndOpportunityName(string Entity, Guid Id, string criteria, IOrganizationService Service, string[] columns)
        {

            QueryExpression queryRetrieveName = new QueryExpression(Entity);
            queryRetrieveName.ColumnSet.AddColumns(columns);
            queryRetrieveName.Criteria.AddCondition(criteria, ConditionOperator.Equal, Id);

            //queryRetrieveAccount.AddLink(Entity, Name, "name");
            //queryRetrieveAccount.LinkEntities[0].Columns.AddColumns("accountid");
            //queryRetrieveAccount.LinkEntities[0].EntityAlias = "accountId";

            return Service.RetrieveMultiple(queryRetrieveName);
        }

        private EntityCollection RetrieveAccountAndOpportunityName(string Entity, Guid Id, string Name, IOrganizationService Service)
        {

            QueryExpression queryRetrieveName = new QueryExpression(Entity);
            queryRetrieveName.ColumnSet.AddColumns(Name);
            queryRetrieveName.Criteria.AddCondition(Name, ConditionOperator.Equal, Id);

            //queryRetrieveAccount.AddLink(Entity, Name, "name");
            //queryRetrieveAccount.LinkEntities[0].Columns.AddColumns("accountid");
            //queryRetrieveAccount.LinkEntities[0].EntityAlias = "accountId";

            return Service.RetrieveMultiple(queryRetrieveName);
        }

        private EntityCollection RetrieveId(string Entity, string Id, string Name, IOrganizationService Service)
        {
            QueryExpression queryRetrieveId = new QueryExpression(Entity);
            queryRetrieveId.ColumnSet.AddColumns(Id);
            queryRetrieveId.Criteria.AddCondition("name", ConditionOperator.Equal, Name);

            return Service.RetrieveMultiple(queryRetrieveId);
        }

    }
}