using System;

namespace Ach.Fulfillment.Tests.Business
{
    using System.Collections.ObjectModel;
    using Data;
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
                DfiAccountId = "12345678901234567",
                Amount = (decimal)123.00,
                CallbackUrl = "test.com",
                TransactionStatus = AchTransactionStatus.Received,
                EntryDescription = "PAYROLL",
                IndividualIdNumber = "1234567890",
                ReceiverName = "SomeName",
                TransitRoutingNumber = "123456789",
                EntryClassCode = "PPD",
                ServiceClassCode = "200",
                TransactionCode = "22",
                PaymentRelatedInfo = "dsdfdsfsdf",
                EntryDate = DateTime.Now
            };

            return transaction;

        }
    }
}