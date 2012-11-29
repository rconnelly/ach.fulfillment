namespace Ach.Fulfillment.Tests.Business
{
    using System;
    using System.Collections.Generic;
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

        public static AchTransactionEntity CreateTestAchTransaction(this BusinessIntegrationTestBase testBase)
        {
            var transaction = new AchTransactionEntity
            {
                DfiAccountId = "12345678901234567",
                Amount = (decimal)123.00,
                CallbackUrl = "http://test.com",
                TransactionStatus = AchTransactionStatus.Received,
                EntryDescription = "PAYROLL",
                IndividualIdNumber = "123456789012345",
                ReceiverName = "SomeName",
                TransitRoutingNumber = "123456789",
                EntryClassCode = "PPD",
                ServiceClassCode = "200",
                TransactionCode = "22",
                EntryDate = DateTime.Now
            };

            return transaction;
        }

        public static AchFileEntity CreateTestAchFile(this BusinessIntegrationTestBase testBase)
        {
            var achFile = new AchFileEntity
            {
                Name = "AchFilename",
                FileIdModifier = "A",
                FileStatus = AchFileStatus.Created,
                Locked = false,
                Transactions = new List<AchTransactionEntity>()
            };

            return achFile;
        }
    }
}