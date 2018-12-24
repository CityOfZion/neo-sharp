using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NeoSharp.Core.Blockchain.Repositories;
using NeoSharp.Core.Models;
using NeoSharp.Core.Models.OperationManager;
using NeoSharp.Core.Types;
using NeoSharp.TestHelpers;
using NeoSharp.Types;

namespace NeoSharp.Core.Test.Models
{
    [TestClass]
    public class UtTransactionOperationManager : TestBase
    {
        [TestMethod]
        public async Task Verify_AttributeUsageECDH02()
        {
            var testee = AutoMockContainer.Create<TransactionOperationManager>();

            var transaction = new EnrollmentTransaction
            {
                Attributes = new []
                {
                    new TransactionAttribute
                    {
                        Usage = TransactionAttributeUsage.ECDH02
                    }
                }
            };
            
            var result = await testee.Verify(transaction);
            
            result.Should().BeFalse();
        }

        [TestMethod]
        public async Task Verify_WithInputsWithSamePrevHashAndPrevIndex()
        {
            var testee = AutoMockContainer.Create<TransactionOperationManager>();

            var transaction = new Transaction
            {
                Attributes = new []
                {
                    new TransactionAttribute
                    {
                        Usage = TransactionAttributeUsage.ContractHash
                    }
                },
                Inputs = new[]
                {
                    new CoinReference
                    {
                        PrevHash = UInt256.Zero,
                        PrevIndex = 1
                    },
                    new CoinReference
                    {
                        PrevHash = UInt256.Zero,
                        PrevIndex = 1
                    }
                }
            };

            var result = await testee.Verify(transaction);
            
            result.Should().BeFalse();
        }

        [TestMethod]
        public async Task Verify_WithDoubleSpending()
        {
            var testee = AutoMockContainer.Create<TransactionOperationManager>();

            var transaction = new Transaction
            {
                Attributes = new []
                {
                    new TransactionAttribute
                    {
                        Usage = TransactionAttributeUsage.ContractHash
                    }
                },
                Inputs = new[]
                {
                    new CoinReference
                    {
                        PrevHash = UInt256.Zero,
                        PrevIndex = 1
                    },
                    new CoinReference
                    {
                        PrevHash = UInt256.Zero,
                        PrevIndex = 2
                    }
                }
            };
            
            AutoMockContainer.GetMock<ITransactionRepository>()
                .Setup(b => b.IsDoubleSpend(transaction))
                .Returns(true);

            var result = await testee.Verify(transaction);
            
            result.Should().BeFalse();
        }

        [TestMethod]
        public async Task Verify_WithStrangeAssetId()
        {
            var testee = AutoMockContainer.Create<TransactionOperationManager>();

            var transaction = new Transaction
            {
                Attributes = new []
                {
                    new TransactionAttribute
                    {
                        Usage = TransactionAttributeUsage.ContractHash
                    }
                },
                Inputs = new[]
                {
                    new CoinReference
                    {
                        PrevHash = UInt256.Zero,
                        PrevIndex = 1
                    },
                    new CoinReference
                    {
                        PrevHash = UInt256.Zero,
                        PrevIndex = 2
                    }
                },
                Outputs = new[]
                {
                    new TransactionOutput
                    {
                        AssetId = UInt256.Zero
                    }
                }
            };

            AutoMockContainer
                .GetMock<ITransactionRepository>()
                .Setup(b => b.IsDoubleSpend(transaction))
                .Returns(false);

            AutoMockContainer
                .GetMock<IAssetRepository>()
                .Setup(b => b.GetAsset(It.IsAny<UInt256>()))
                .ReturnsAsync(() => null);

            var result = await testee.Verify(transaction);
            
            result.Should().BeFalse();
        }

        [TestMethod]
        public async Task Verify_WithKnownAssetIdButNotGeverningAndNotUtility()
        {
            var testee = AutoMockContainer.Create<TransactionOperationManager>();

            var transaction = new Transaction
            {
                Attributes = new []
                {
                    new TransactionAttribute
                    {
                        Usage = TransactionAttributeUsage.ContractHash
                    }
                },
                Inputs = new[]
                {
                    new CoinReference
                    {
                        PrevHash = UInt256.Zero,
                        PrevIndex = 1
                    },
                    new CoinReference
                    {
                        PrevHash = UInt256.Zero,
                        PrevIndex = 2
                    }
                },
                Outputs = new[]
                {
                    new TransactionOutput
                    {
                        AssetId = UInt256.Zero
                    }
                }
            };

            AutoMockContainer
                .GetMock<ITransactionRepository>()
                .Setup(b => b.IsDoubleSpend(transaction))
                .Returns(false);

            AutoMockContainer
                .GetMock<IAssetRepository>()
                .Setup(b => b.GetAsset(It.IsAny<UInt256>()))
                .ReturnsAsync(() => new Asset
                {
                    AssetType = AssetType.DutyFlag
                });

            var result = await testee.Verify(transaction);
            
            result.Should().BeFalse();
        }

        [TestMethod]
        public async Task Verify_WithOutputValueDivisibleByAssetRule()
        {
            var testee = AutoMockContainer.Create<TransactionOperationManager>();

            var transaction = new Transaction
            {
                Attributes = new []
                {
                    new TransactionAttribute
                    {
                        Usage = TransactionAttributeUsage.ContractHash
                    }
                },
                Inputs = new[]
                {
                    new CoinReference
                    {
                        PrevHash = UInt256.Zero,
                        PrevIndex = 1
                    },
                    new CoinReference
                    {
                        PrevHash = UInt256.Zero,
                        PrevIndex = 2
                    }
                },
                Outputs = new[]
                {
                    new TransactionOutput
                    {
                        AssetId = UInt256.Zero,
                        Value = Fixed8.MaxValue
                    }
                }
            };

            AutoMockContainer
                .GetMock<ITransactionRepository>()
                .Setup(b => b.IsDoubleSpend(transaction))
                .Returns(false);

            AutoMockContainer
                .GetMock<IAssetRepository>()
                .Setup(b => b.GetAsset(It.IsAny<UInt256>()))
                .ReturnsAsync(() => new Asset
                {
                    AssetType = AssetType.GoverningToken
                });

            var result = await testee.Verify(transaction);
            
            result.Should().BeFalse();
        }

        [TestMethod]
        public async Task Verify_WithoutReferences()
        {
            var testee = AutoMockContainer.Create<TransactionOperationManager>();

            var transaction = new Transaction
            {
                Attributes = new []
                {
                    new TransactionAttribute
                    {
                        Usage = TransactionAttributeUsage.ContractHash
                    }
                },
                Inputs = new[]
                {
                    new CoinReference
                    {
                        PrevHash = UInt256.Zero,
                        PrevIndex = 1
                    },
                    new CoinReference
                    {
                        PrevHash = UInt256.Zero,
                        PrevIndex = 2
                    }
                },
                Outputs = new[]
                {
                    new TransactionOutput
                    {
                        AssetId = UInt256.Zero,
                        Value = Fixed8.One
                    }
                }
            };

            var transactionModelMock = AutoMockContainer.GetMock<ITransactionRepository>();
            transactionModelMock
                .Setup(b => b.IsDoubleSpend(transaction))
                .Returns(false);
            transactionModelMock
                .Setup(x => x.GetTransaction(It.IsAny<UInt256>()))
                .ReturnsAsync(() => null);

            AutoMockContainer
                .GetMock<IAssetRepository>()
                .Setup(b => b.GetAsset(It.IsAny<UInt256>()))
                .ReturnsAsync(() => new Asset
                {
                    AssetType = AssetType.GoverningToken
                });

            var result = await testee.Verify(transaction);
            
            result.Should().BeFalse();
        }

        [TestMethod]
        public async Task Verify_WithMoreThanOneReferenceAmountGreaterThanZero()
        {
            var testee = AutoMockContainer.Create<TransactionOperationManager>();

            var transaction = new Transaction
            {
                Attributes = new []
                {
                    new TransactionAttribute
                    {
                        Usage = TransactionAttributeUsage.ContractHash
                    }
                },
                Inputs = new[]
                {
                    new CoinReference
                    {
                        PrevHash = UInt256.Zero,
                        PrevIndex = 1
                    },
                    new CoinReference
                    {
                        PrevHash = UInt256.Zero,
                        PrevIndex = 2
                    }
                },
                Outputs = new[]
                {
                    new TransactionOutput
                    {
                        AssetId = UInt256.Zero,
                        Value = Fixed8.One
                    }
                }
            };

            var transactionOfPreviousHash = new Transaction
            {
                Outputs = new []
                {
                    new TransactionOutput(), // it's not using the first because PrevIndex is 1
                    new TransactionOutput
                    {
                        AssetId = UInt256.Parse("1a259dba256600620c6c91094f3a300b30f0cbaecee19c6114deffd3288957d7"),
                        Value = Fixed8.One
                    },
                    new TransactionOutput
                    {
                        AssetId = UInt256.Parse("d4dab99ed65c3655a9619b215ab1988561b706b6e5196b6e0ada916aa6601622"),
                        Value = Fixed8.One
                    }
                }
            };

            var transactionModelMock = AutoMockContainer.GetMock<ITransactionRepository>();
            transactionModelMock
                .Setup(b => b.IsDoubleSpend(transaction))
                .Returns(false);
            transactionModelMock
                .Setup(x => x.GetTransaction(It.IsAny<UInt256>()))
                .ReturnsAsync(() => transactionOfPreviousHash);

            AutoMockContainer
                .GetMock<IAssetRepository>()
                .Setup(b => b.GetAsset(It.IsAny<UInt256>()))
                .ReturnsAsync(() => new Asset
                {
                    AssetType = AssetType.GoverningToken
                });

            var result = await testee.Verify(transaction);
            
            result.Should().BeFalse();
        }

        [TestMethod]
        public async Task Verify_WithOnlyOneReferenceAmountGreaterThanZeroButItsNotUtilityToken()
        {
            var testee = AutoMockContainer.Create<TransactionOperationManager>();

            var transaction = new Transaction
            {
                Attributes = new []
                {
                    new TransactionAttribute
                    {
                        Usage = TransactionAttributeUsage.ContractHash
                    }
                },
                Inputs = new[]
                {
                    new CoinReference
                    {
                        PrevHash = UInt256.Zero,
                        PrevIndex = 1
                    },
                    new CoinReference
                    {
                        PrevHash = UInt256.Zero,
                        PrevIndex = 2
                    }
                },
                Outputs = new[]
                {
                    new TransactionOutput
                    {
                        AssetId = UInt256.Zero,
                        Value = Fixed8.One
                    }
                }
            };

            var transactionOfPreviousHash = new Transaction
            {
                Outputs = new []
                {
                    new TransactionOutput(), // it's not using the first because PrevIndex is 1
                    new TransactionOutput
                    {
                        AssetId = UInt256.Parse("1a259dba256600620c6c91094f3a300b30f0cbaecee19c6114deffd3288957d7"),
                        Value = Fixed8.One
                    },
                    new TransactionOutput
                    {
                        AssetId = UInt256.Parse("1a259dba256600620c6c91094f3a300b30f0cbaecee19c6114deffd3288957d7"),
                        Value = Fixed8.One
                    }
                }
            };

            var transactionModelMock = AutoMockContainer.GetMock<ITransactionRepository>();
            transactionModelMock
                .Setup(b => b.IsDoubleSpend(transaction))
                .Returns(false);
            transactionModelMock
                .Setup(x => x.GetTransaction(It.IsAny<UInt256>()))
                .ReturnsAsync(() => transactionOfPreviousHash);

            AutoMockContainer
                .GetMock<IAssetRepository>()
                .Setup(b => b.GetAsset(It.IsAny<UInt256>()))
                .ReturnsAsync(() => new Asset
                {
                    AssetType = AssetType.GoverningToken
                });

            AutoMockContainer
                .GetMock<ITransactionContext>()
                .SetupGet(x => x.UtilityTokenHash)
                .Returns(UInt256.Parse("602c79718b16e442de58778e148d0b1084e3b2dffd5de6b7b16cee7969282de7"));
            
            var result = await testee.Verify(transaction);
            
            result.Should().BeFalse();
        }

        [TestMethod]
        public async Task Verify_WithReferenceAmountZeroAndExistingSystemFee()
        {
            var testee = AutoMockContainer.Create<TransactionOperationManager>();

            var transaction = new Transaction
            {
                Attributes = new []
                {
                    new TransactionAttribute
                    {
                        Usage = TransactionAttributeUsage.ContractHash
                    }
                },
                Inputs = new[]
                {
                    new CoinReference
                    {
                        PrevHash = UInt256.Zero,
                        PrevIndex = 1
                    },
                    new CoinReference
                    {
                        PrevHash = UInt256.Zero,
                        PrevIndex = 2
                    }
                },
                Outputs = new[]
                {
                    new TransactionOutput
                    {
                        AssetId = UInt256.Zero,
                        Value = Fixed8.One
                    }
                }
            };
            
            var transactionOfPreviousHash = new Transaction
            {
                Outputs = new []
                {
                    new TransactionOutput(), // it's not using the first because PrevIndex is 1
                    new TransactionOutput
                    {
                        AssetId = UInt256.Parse("602c79718b16e442de58778e148d0b1084e3b2dffd5de6b7b16cee7969282de7"),
                        Value = Fixed8.Zero
                    },
                    new TransactionOutput
                    {
                        AssetId = UInt256.Parse("602c79718b16e442de58778e148d0b1084e3b2dffd5de6b7b16cee7969282de7"),
                        Value = Fixed8.Zero
                    }
                }
            };

            var transactionModelMock = AutoMockContainer.GetMock<ITransactionRepository>();
            transactionModelMock
                .Setup(b => b.IsDoubleSpend(transaction))
                .Returns(false);
            transactionModelMock
                .Setup(x => x.GetTransaction(It.IsAny<UInt256>()))
                .ReturnsAsync(() => transactionOfPreviousHash);

            AutoMockContainer
                .GetMock<IAssetRepository>()
                .Setup(b => b.GetAsset(It.IsAny<UInt256>()))
                .ReturnsAsync(() => new Asset
                {
                    AssetType = AssetType.GoverningToken
                });

            var transactionContextMock = AutoMockContainer.GetMock<ITransactionContext>();
            transactionContextMock
                .SetupGet(x => x.UtilityTokenHash)
                .Returns(UInt256.Parse("602c79718b16e442de58778e148d0b1084e3b2dffd5de6b7b16cee7969282de7"));
            transactionContextMock
                .Setup(x => x.GetSystemFee(It.IsAny<Transaction>()))
                .Returns(Fixed8.One);
            
            var result = await testee.Verify(transaction);
            
            result.Should().BeFalse();
        }

        [TestMethod]
        public async Task Verify_WithReferenceAmountLessThanSystemFee()
        {
            var testee = AutoMockContainer.Create<TransactionOperationManager>();

            var transaction = new Transaction
            {
                Attributes = new []
                {
                    new TransactionAttribute
                    {
                        Usage = TransactionAttributeUsage.ContractHash
                    }
                },
                Inputs = new[]
                {
                    new CoinReference
                    {
                        PrevHash = UInt256.Zero,
                        PrevIndex = 1
                    },
                    new CoinReference
                    {
                        PrevHash = UInt256.Zero,
                        PrevIndex = 2
                    }
                },
                Outputs = new[]
                {
                    new TransactionOutput
                    {
                        AssetId = UInt256.Zero,
                        Value = Fixed8.One
                    }
                }
            };

            var transactionOfPreviousHash = new Transaction
            {
                Outputs = new []
                {
                    new TransactionOutput(), // it's not using the first because PrevIndex is 1
                    new TransactionOutput
                    {
                        AssetId = UInt256.Parse("602c79718b16e442de58778e148d0b1084e3b2dffd5de6b7b16cee7969282de7"),
                        Value = Fixed8.One
                    },
                    new TransactionOutput
                    {
                        AssetId = UInt256.Parse("602c79718b16e442de58778e148d0b1084e3b2dffd5de6b7b16cee7969282de7"),
                        Value = Fixed8.One
                    }
                }
            };

            var transactionModelMock = AutoMockContainer.GetMock<ITransactionRepository>();
            transactionModelMock
                .Setup(b => b.IsDoubleSpend(transaction))
                .Returns(false);
            transactionModelMock
                .Setup(x => x.GetTransaction(It.IsAny<UInt256>()))
                .ReturnsAsync(() => transactionOfPreviousHash);

            AutoMockContainer
                .GetMock<IAssetRepository>()
                .Setup(b => b.GetAsset(It.IsAny<UInt256>()))
                .ReturnsAsync(() => new Asset
                {
                    AssetType = AssetType.GoverningToken
                });

            var transactionContextMock = AutoMockContainer.GetMock<ITransactionContext>();
            transactionContextMock
                .SetupGet(x => x.UtilityTokenHash)
                .Returns(UInt256.Parse("602c79718b16e442de58778e148d0b1084e3b2dffd5de6b7b16cee7969282de7"));
            transactionContextMock
                .Setup(x => x.GetSystemFee(It.IsAny<Transaction>()))
                .Returns(new Fixed8(300000000));

            var result = await testee.Verify(transaction);
            
            result.Should().BeFalse();
        }

        [TestMethod]
        public async Task Verify_ClaimTransacWithNegativeResultOfUtilityToken()
        {
            var testee = AutoMockContainer.Create<TransactionOperationManager>();

            var transaction = new ClaimTransaction
            {
                Attributes = new []
                {
                    new TransactionAttribute
                    {
                        Usage = TransactionAttributeUsage.ContractHash
                    }
                },
                Inputs = new[]
                {
                    new CoinReference
                    {
                        PrevHash = UInt256.Zero,
                        PrevIndex = 1
                    },
                    new CoinReference
                    {
                        PrevHash = UInt256.Zero,
                        PrevIndex = 2
                    }
                },
                Outputs = new[]
                {
                    new TransactionOutput
                    {
                        AssetId = UInt256.Zero,
                        Value = Fixed8.One
                    }
                }
            };

            var transactionOfPreviousHash = new Transaction
            {
                Outputs = new []
                {
                    new TransactionOutput(), // it's not using the first because PrevIndex is 1
                    new TransactionOutput
                    {
                        AssetId = UInt256.Parse("602c79718b16e442de58778e148d0b1084e3b2dffd5de6b7b16cee7969282de7"),
                        Value = new Fixed8(-5)
                    }
                }
            };

            var transactionModelMock = AutoMockContainer.GetMock<ITransactionRepository>();
            transactionModelMock
                .Setup(b => b.IsDoubleSpend(transaction))
                .Returns(false);
            transactionModelMock
                .Setup(x => x.GetTransaction(It.IsAny<UInt256>()))
                .ReturnsAsync(() => transactionOfPreviousHash);

            AutoMockContainer
                .GetMock<IAssetRepository>()
                .Setup(b => b.GetAsset(It.IsAny<UInt256>()))
                .ReturnsAsync(() => new Asset
                {
                    AssetType = AssetType.GoverningToken
                });

            var transactionContextMock = AutoMockContainer.GetMock<ITransactionContext>();
            transactionContextMock
                .SetupGet(x => x.UtilityTokenHash)
                .Returns(UInt256.Parse("602c79718b16e442de58778e148d0b1084e3b2dffd5de6b7b16cee7969282de7"));
            transactionContextMock
                .Setup(x => x.GetSystemFee(It.IsAny<Transaction>()))
                .Returns(Fixed8.Zero);

            var result = await testee.Verify(transaction);
            
            result.Should().BeFalse();
        }

        [TestMethod]
        public async Task Verify_NotMinerTransacWithNegativeResults()
        {
            var testee = AutoMockContainer.Create<TransactionOperationManager>();

            var transaction = new EnrollmentTransaction
            {
                Attributes = new []
                {
                    new TransactionAttribute
                    {
                        Usage = TransactionAttributeUsage.ContractHash
                    }
                },
                Inputs = new[]
                {
                    new CoinReference
                    {
                        PrevHash = UInt256.Zero,
                        PrevIndex = 1
                    },
                    new CoinReference
                    {
                        PrevHash = UInt256.Zero,
                        PrevIndex = 2
                    }
                },
                Outputs = new[]
                {
                    new TransactionOutput
                    {
                        AssetId = UInt256.Zero,
                        Value = Fixed8.One
                    }
                }
            };

            var transactionOfPreviousHash = new Transaction
            {
                Outputs = new []
                {
                    new TransactionOutput(), // it's not using the first because PrevIndex is 1
                    new TransactionOutput
                    {
                        AssetId = UInt256.Parse("602c79718b16e442de58778e148d0b1084e3b2dffd5de6b7b16cee7969282de7"),
                        Value = Fixed8.One
                    },
                    new TransactionOutput
                    {
                        AssetId = UInt256.Parse("602c79718b16e442de58778e148d0b1084e3b2dffd5de6b7b16cee7969282de7"),
                        Value = Fixed8.One
                    }
                }
            };

            var transactionModelMock = AutoMockContainer.GetMock<ITransactionRepository>();
            transactionModelMock
                .Setup(b => b.IsDoubleSpend(transaction))
                .Returns(false);
            transactionModelMock
                .Setup(x => x.GetTransaction(It.IsAny<UInt256>()))
                .ReturnsAsync(() => transactionOfPreviousHash);

            AutoMockContainer
                .GetMock<IAssetRepository>()
                .Setup(b => b.GetAsset(It.IsAny<UInt256>()))
                .ReturnsAsync(() => new Asset
                {
                    AssetType = AssetType.GoverningToken
                });

            var transactionContextMock = AutoMockContainer.GetMock<ITransactionContext>();
            transactionContextMock
                .SetupGet(x => x.UtilityTokenHash)
                .Returns(UInt256.Parse("602c79718b16e442de58778e148d0b1084e3b2dffd5de6b7b16cee7969282de7"));
            transactionContextMock
                .Setup(x => x.GetSystemFee(It.IsAny<Transaction>()))
                .Returns(new Fixed8(190000000));

            var result = await testee.Verify(transaction);
            
            result.Should().BeFalse();
        }

        [TestMethod]
        public async Task Verify_WitnessVerifiedWrong()
        {
            var testee = AutoMockContainer.Create<TransactionOperationManager>();

            var transaction = new EnrollmentTransaction
            {
                Attributes = new []
                {
                    new TransactionAttribute
                    {
                        Usage = TransactionAttributeUsage.ContractHash
                    }
                },
                Inputs = new[]
                {
                    new CoinReference
                    {
                        PrevHash = UInt256.Zero,
                        PrevIndex = 1
                    },
                    new CoinReference
                    {
                        PrevHash = UInt256.Zero,
                        PrevIndex = 2
                    }
                },
                Outputs = new[]
                {
                    new TransactionOutput
                    {
                        AssetId = UInt256.Parse("1a259dba256600620c6c91094f3a300b30f0cbaecee19c6114deffd3288957d7"),
                        Value = new Fixed8(200000000)
                    }
                },
                Witness = new []
                {
                    new Witness()
                }
            };

            var transactionOfPreviousHash = new Transaction
            {
                Outputs = new []
                {
                    new TransactionOutput(), // it's not using the first because PrevIndex is 1
                    new TransactionOutput
                    {
                        AssetId = UInt256.Parse("602c79718b16e442de58778e148d0b1084e3b2dffd5de6b7b16cee7969282de7"),
                        Value = new Fixed8(200000000)
                    },
                    new TransactionOutput
                    {
                        AssetId = UInt256.Parse("1a259dba256600620c6c91094f3a300b30f0cbaecee19c6114deffd3288957d7"),
                        Value = new Fixed8(200000000)
                    }
                }
            };

            var transactionModelMock = AutoMockContainer.GetMock<ITransactionRepository>();
            transactionModelMock
                .Setup(b => b.IsDoubleSpend(transaction))
                .Returns(false);
            transactionModelMock
                .Setup(x => x.GetTransaction(It.IsAny<UInt256>()))
                .ReturnsAsync(() => transactionOfPreviousHash);

            AutoMockContainer
                .GetMock<IAssetRepository>()
                .Setup(b => b.GetAsset(It.IsAny<UInt256>()))
                .ReturnsAsync(() => new Asset
                {
                    AssetType = AssetType.GoverningToken
                });

            var transactionContextMock = AutoMockContainer.GetMock<ITransactionContext>();
            transactionContextMock
                .SetupGet(x => x.UtilityTokenHash)
                .Returns(UInt256.Parse("602c79718b16e442de58778e148d0b1084e3b2dffd5de6b7b16cee7969282de7"));
            transactionContextMock
                .Setup(x => x.GetSystemFee(It.IsAny<Transaction>()))
                .Returns(new Fixed8(190000000));

            AutoMockContainer
                .GetMock<IWitnessOperationsManager>()
                .Setup(x => x.Verify(It.IsAny<Witness>()))
                .Returns(Task.FromResult(false));
            
            var result = await testee.Verify(transaction);
            
            result.Should().BeFalse();
        }

        [TestMethod]
        public async Task Verify_Success()
        {
            var testee = AutoMockContainer.Create<TransactionOperationManager>();

            var transaction = new EnrollmentTransaction
            {
                Attributes = new []
                {
                    new TransactionAttribute
                    {
                        Usage = TransactionAttributeUsage.ContractHash
                    }
                },
                Inputs = new[]
                {
                    new CoinReference
                    {
                        PrevHash = UInt256.Zero,
                        PrevIndex = 1
                    },
                    new CoinReference
                    {
                        PrevHash = UInt256.Zero,
                        PrevIndex = 2
                    }
                },
                Outputs = new[]
                {
                    new TransactionOutput
                    {
                        AssetId = UInt256.Parse("1a259dba256600620c6c91094f3a300b30f0cbaecee19c6114deffd3288957d7"),
                        Value = new Fixed8(200000000)
                    }
                },
                Witness = new []
                {
                    new Witness()
                }
            };

            var transactionOfPreviousHash = new Transaction
            {
                Outputs = new []
                {
                    new TransactionOutput(), // it's not using the first because PrevIndex is 1
                    new TransactionOutput
                    {
                        AssetId = UInt256.Parse("602c79718b16e442de58778e148d0b1084e3b2dffd5de6b7b16cee7969282de7"),
                        Value = new Fixed8(200000000)
                    },
                    new TransactionOutput
                    {
                        AssetId = UInt256.Parse("1a259dba256600620c6c91094f3a300b30f0cbaecee19c6114deffd3288957d7"),
                        Value = new Fixed8(200000000)
                    }
                }
            };

            var transactionModelMock = AutoMockContainer.GetMock<ITransactionRepository>();
            transactionModelMock
                .Setup(b => b.IsDoubleSpend(transaction))
                .Returns(false);
            transactionModelMock
                .Setup(x => x.GetTransaction(It.IsAny<UInt256>()))
                .ReturnsAsync(() => transactionOfPreviousHash);

            AutoMockContainer
                .GetMock<IAssetRepository>()
                .Setup(b => b.GetAsset(It.IsAny<UInt256>()))
                .ReturnsAsync(() => new Asset
                {
                    AssetType = AssetType.GoverningToken
                });

            var transactionContextMock = AutoMockContainer.GetMock<ITransactionContext>();
            transactionContextMock
                .SetupGet(x => x.UtilityTokenHash)
                .Returns(UInt256.Parse("602c79718b16e442de58778e148d0b1084e3b2dffd5de6b7b16cee7969282de7"));
            transactionContextMock
                .Setup(x => x.GetSystemFee(It.IsAny<Transaction>()))
                .Returns(new Fixed8(190000000));

            AutoMockContainer
                .GetMock<IWitnessOperationsManager>()
                .Setup(x => x.Verify(It.IsAny<Witness>()))
                .Returns(Task.FromResult(true));

            var result = await testee.Verify(transaction);

            result.Should().BeTrue();
        }
    }
}