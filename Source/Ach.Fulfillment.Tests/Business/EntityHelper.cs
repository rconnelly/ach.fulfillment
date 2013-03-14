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
                CallbackUrl = "http://ya.ru",
                Status = AchTransactionStatus.Created,
                EntryDescription = "PAYROLL",
                IndividualIdNumber = "123456789012345",
                ReceiverName = "SomeName",
                TransitRoutingNumber = "123456789",
                EntryClassCode = "PPD",
                ServiceClassCode = 200,
                TransactionCode = 22,
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
                    Transactions = new List<AchTransactionEntity>()
                };

            return achFile;
        }

        public static PartnerEntity CreateTestPartnerWithDetails(this BusinessIntegrationTestBase testBase)
        {
            var partner = new PartnerEntity
            {
                Name = ShortStringGenerator.GetRandomValue()
            };

            var details = new PartnerDetailEntity
                {
                    CompanyIdentification = new StringGenerator(10, 10).GetRandomValue(),
                    CompanyName = new StringGenerator(5, 16).GetRandomValue(),
                    DfiIdentification = new StringGenerator(8, 8).GetRandomValue(),
                    Destination = new StringGenerator(5, 23).GetRandomValue(),
                    ImmediateDestination = new StringGenerator(5, 10).GetRandomValue(),
                    OriginOrCompanyName = new StringGenerator(5, 23).GetRandomValue(),
                    Partner = partner
                };

            partner.Details = details;

            return partner;
        }
    }
}