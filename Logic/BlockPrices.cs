


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

        public static decimal GetCombinedEnergyPricePerKWH(CalculationOptions calculationOptions, int block)
        {
            if (calculationOptions.IncludeVAT)
            {
                return UserGroupAndBlockPricesCombined[calculationOptions.UserGroup][block].PricePerKWh * 1.22M;
            }
            else
            {
                return UserGroupAndBlockPricesCombined[calculationOptions.UserGroup][block].PricePerKWh;
            }
        }

        public static decimal GetCombinedPowerPricePerKW(CalculationOptions calculationOptions, int block)
        {
            if (calculationOptions.IncludeVAT)
            {
                return UserGroupAndBlockPricesCombined[calculationOptions.UserGroup][block].PricePerKW * 1.22M;
            }
            else
            {
                return UserGroupAndBlockPricesCombined[calculationOptions.UserGroup][block].PricePerKW;
            }
        }


        public static decimal GetFixedPricePerKW(CalculationOptions calculationOptions)
        {
            if (calculationOptions.IncludeVAT)
            {
                return CalculationOptions.OldPriceList[calculationOptions.ConnectionType][calculationOptions.VrstaOdjema].OM * 1.22M;
            }
            return CalculationOptions.OldPriceList[calculationOptions.ConnectionType][calculationOptions.VrstaOdjema].OM;
        }

        internal static decimal GetOldTransferEnergyPriceSingleTariffPerKWh(CalculationOptions calculationOptions)
        {
            var price = CalculationOptions.OldPriceList[calculationOptions.ConnectionType][calculationOptions.VrstaOdjema].ET;
            if (calculationOptions.IncludeVAT)
            {
                return price * 1.22M;
            }
            return price;
        }

        public static decimal GetOldTransferEnergyPricePerKWH(bool? highTariff, CalculationOptions calculationOptions)
        {
            var priceList = CalculationOptions.OldPriceList[calculationOptions.ConnectionType][calculationOptions.VrstaOdjema];
            var tariff = highTariff switch {
                true => priceList.VT,
                false => priceList.MT,
                null => priceList.ET,
            };
            if (calculationOptions.IncludeVAT)
            {
                return tariff * 1.22M;
            }
            return tariff;
        }

        internal static decimal GetNonBlockTransferEnergyPerKWH(bool? highTariff, CalculationOptions calculationOptions)
        {
            var tariff = highTariff switch {
                true => 0.00623M + 0.01245M,
                false => 0.00593M + 0.01246M,
                null => 0.00607M + 0.01246M,
            };
            if (calculationOptions.IncludeVAT)
            {
                return tariff * 1.22M;
            }
            return tariff;
        }
        internal static decimal GetCombinedPowerPriceNo15Minutes(CalculationOptions calculationOptions)
        {
            var price = 0.14326M + 2.14808M;
            if (calculationOptions.BreakersValue.ThreePhase)
            {
                if (calculationOptions.BreakersValue.PrikljucnaMoc > 17)
                {
                    price *= calculationOptions.BreakersValue.PrikljucnaMoc * 0.62M;
                }
                else
                {
                    price *= calculationOptions.BreakersValue.PrikljucnaMoc * 0.42M;
                }
            }
            else
            {
                price *= calculationOptions.BreakersValue.PrikljucnaMoc * 0.58M;
            }
            if (calculationOptions.IncludeVAT)
            {
                return price * 1.22M;
            }
            return price;
        }

        private static (decimal PricePerKW, decimal PricePerKWh)[][] UserGroupAndBlockPricesCombined { get; } = [
            [
                (3.61324M, 0.01958M),
                (0.88240M, 0.01844M),
                (0.19137M, 0.01837M),
                (0.01316M, 0.01838M),
                (0.00000M, 0.01847M),
            ],
            [
                (5.33444M, 0.01454M),
                (1.08944M, 0.01389M),
                (0.14257M, 0.01369M),
                (0.00368M, 0.01330M),
                (0.00000M, 0.01329M),
            ],
            [
                (4.18586M, 0.01263M),
                (0.88405M, 0.01204M),
                (0.11318M, 0.01181M),
                (0.00107M, 0.01140M),
                (0.00000M, 0.01139M),
            ],
            [
                (1.95873M, 0.00810M),
                (0.44459M, 0.00797M),
                (0.07189M, 0.00762M),
                (0.00140M, 0.00742M),
                (0.00000M, 0.00736M),
            ],
            [
                (0.56683M, 0.00829M),
                (0.25891M, 0.00813M),
                (0.05109M, 0.00776M),
                (0.00186M, 0.00753M),
                (0.00000M, 0.00748M),
            ]];
    }
}
