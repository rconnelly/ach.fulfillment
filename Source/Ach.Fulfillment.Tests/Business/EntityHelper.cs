using System;

namespace Ach.Fulfillment.Tests.Business
{
    using System.Collections.ObjectModel;

    using Ach.Fulfillment.Data;

    using QuickGenerate.Primitives;

    internal static class EntityHelper
    {
        private static readonly StringGenerator ShortStringGenerator = new StringGenerator(5, 20);

        public static RoleEntity CreateTestRole(this BusinessIntegrationTestBase testBase)
        {
            var role = new RoleEntity { Name = ShortStringGenerator.GetRandomValue(), };
            role.Permissions = new Collection<PermissionEntity>
                {
                    new PermissionEntity
                        {
                            Name = AccessRight.Admin,
                            Role = role
                        }
                };

            return role;
        }

        public static PartnerEntity CreateTestPartner(this BusinessIntegrationTestBase testBase)
        {
            var p = new PartnerEntity
                        {
                            Name = ShortStringGenerator.GetRandomValue(),
                        };
            p.Details = new PartnerDetailEntity(p);
            return p;
        }

        public static UserEntity CreateTestUser(this BusinessIntegrationTestBase testBase)
        {
            var u = new UserEntity
                {
                    Name = ShortStringGenerator.GetRandomValue(),
                    Email = "aaa@gmail.com",
                };

            var r = CreateTestRole(testBase);
            u.Role = r;

            return u;
        }

        public static AchTransactionEntity CreateTestTransaction(this BusinessIntegrationTestBase testBase)
        {
            var transaction = new AchTransactionEntity
            {
                DFIAccountId = "gfhfg67567",
                Amount = (decimal)123.00,
                CallbackUrl = "test.com",
                TransactionStatus = TransactionStatus.Received,
                EntryDescription = "description",
                IndividualIdNumber = "1234567890",
                ReceiverName = "SomeName",
                TransitRoutingNumber = "1230987645",
                EntryClassCode = "PPD",
                ServiceClassCode = "200",
                TransactionCode = "22",
                PaymentRelatedInfo = "dsdfdsfsdf",
                EntryDate = DateTime.UtcNow
            };

            var partner = CreateTestPartner(testBase);
            transaction.Partner = partner;

            return transaction;

        }
    }
}