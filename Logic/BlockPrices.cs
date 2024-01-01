


namespace Omreznina.Client.Logic
{
    public static class BlockPrices
    {
        public static decimal GetFactor(int Year)
        {
            if (Year < 2026)
                return 0.90M;
            else if (Year > 2027)
                return 1.20M;
            else
                return 1.05M;
        }

        public static decimal GetDistributionEnergyPricePerKWH(bool vat, int block)
        {
            if (vat)
            {
                return DistributionEnergyPricePerKWH[block] * 1.22M;
            }
            else
            {
                return DistributionEnergyPricePerKWH[block];
            }
        }

        public static decimal GetCombinedEnergyPricePerKWH(bool vat, int block)
        {
            if (vat)
            {
                return (DistributionEnergyPricePerKWH[block] + TransferEnergyPricePerKWH[block]) * 1.22M;
            }
            else
            {
                return DistributionEnergyPricePerKWH[block] + TransferEnergyPricePerKWH[block];
            }
        }

        public static decimal GetCombinedPowerPricePerKW(bool vat, int block)
        {
            if (vat)
            {
                return (DistributionPowerPricePerKW[block] + TransferPowerPricePerKW[block]) * 1.22M;
            }
            else
            {
                return DistributionPowerPricePerKW[block] + TransferPowerPricePerKW[block];
            }
        }

        public static decimal GetDistributionPowerPricePerKW(bool vat, int block)
        {
            if (vat)
            {
                return DistributionPowerPricePerKW[block] * 1.22M;
            }
            else
            {
                return DistributionPowerPricePerKW[block];
            }
        }

        public static decimal GetTransferEnergyPricePerKWH(bool vat, int block)
        {
            if (vat)
            {
                return TransferEnergyPricePerKWH[block] * 1.22M;
            }
            else
            {
                return TransferEnergyPricePerKWH[block];
            }
        }

        public static decimal GetTransferPowerPricePerKW(bool vat, int block)
        {
            if (vat)
            {
                return TransferPowerPricePerKW[block] * 1.22M;
            }
            else
            {
                return TransferPowerPricePerKW[block];
            }
        }

        public static Dictionary<string, (decimal FixedPerKW, decimal LowerTariff, decimal HigherTariff, decimal ConstantTariff)> FixedPriceListPerKW { get; } = new() {
            {"2023.11.1", (0.77417M, 0.04182M, 0.03215M, 0.03858M) }
        };

        public static decimal GetFixedPricePerKW(bool includeVAT, string oldPricelist)
        {
            if (includeVAT)
            {
                return FixedPriceListPerKW[oldPricelist].FixedPerKW * 1.22M;
            }
            return FixedPriceListPerKW[oldPricelist].FixedPerKW;
        }

        public static decimal GetOldTransferEnergyPricePerKWH(bool includeVAT, bool? highTariff, string oldPriceList)
        {
            var priceList = FixedPriceListPerKW[oldPriceList];
            var tariff = highTariff.HasValue ? (highTariff.Value ? priceList.HigherTariff : priceList.LowerTariff) : priceList.ConstantTariff;
            if (includeVAT)
            {
                return tariff * 1.22M;
            }
            return tariff;
        }

        internal static decimal GetCombinedEnergyPriceSingleTariffPerKWH(bool includeVAT)
        {
            // Distro, transfer
            var price = 0.00607M + 0.01246M;
            if (includeVAT)
            {
                return price * 1.22M;
            }
            return price;
        }

        internal static decimal GetOldEnergyPriceSingleTariffPerKWh(CalculationOptions calculationOptions)
        {
            var price = FixedPriceListPerKW[calculationOptions.OldPricelist].ConstantTariff;
            if (calculationOptions.IncludeVAT)
            {
                return price * 1.22M;
            }
            return price;
        }

        private static decimal[] DistributionEnergyPricePerKWH { get; } = new decimal[] {
            0.01295M,
            0.01224M,
            0.01248M,
            0.01246M,
            0.01258M
        };

        private static decimal[] DistributionPowerPricePerKW { get; } = new decimal[] {
            3.36401M,
            0.83363M,
            0.18034M,
            0.01278M,
            0.00000M
        };

        private static decimal[] TransferEnergyPricePerKWH { get; } = new decimal[] {
            0.00663M,
            0.00620M,
            0.00589M,
            0.00592M,
            0.00589M
        };

        private static decimal[] TransferPowerPricePerKW { get; } = new decimal[] {
            0.24923M,
            0.04877M,
            0.01103M,
            0.00038M,
            0.00000M
        };
    }
}
