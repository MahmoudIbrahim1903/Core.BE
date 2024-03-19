using Emeint.Core.BE.Configurations.Application.Mappers;
using Emeint.Core.BE.Configurations.Domain.Enums;
using Emeint.Core.BE.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Configurations.Domain.Model
{
    public class Setting : Entity
    {
        public string id { set; get; }
        public string Key { set; get; }
        public string Value { set; get; }
        public bool IsRequired { get; set; }
        public string Group { set; get; }
        public int? Minimum { set; get; }
        public int? Maximum { set; get; }
        public SettingUser User { set; get; }
        public SettingType SettingType { set; get; }
        public string EnumTypeName { set; get; }
        public string DisplayName { set; get; }
        public string Description { set; get; }
        public bool? ShowInPortal { set; get; }
        public string UnitOfMeasure { set; get; }

        public void Validate(string value)
        {
            if (IsRequired && (string.IsNullOrWhiteSpace(value) || string.IsNullOrWhiteSpace(value)))
            {
                throw new Emeint.Core.BE.Domain.Exceptions.InvalidOperationException("This value can't be empty.", "هذه القيمه يجب ألا تكون فارغة.");
 
            }
            if (SettingType == SettingType.Integer)
            {
                bool isInteger = int.TryParse(value, out int result);
                if (!isInteger)
                    throw new Emeint.Core.BE.Domain.Exceptions.InvalidOperationException
                        ($"{Key} must be an integer value",
                        $"{Key} يجب أن تكون رقم صحيح");

                if (Minimum != null)
                {
                    var min = Convert.ToInt16(Minimum);
                    if (result < min)
                        throw new Emeint.Core.BE.Domain.Exceptions.InvalidOperationException
                            ($"{Key} must be greater than {Minimum}",
                            $"{Key} يجب أن تكون أكبر من {Minimum}");
                }
                if (Maximum != null)
                {
                    var max = Convert.ToInt16(Maximum);
                    if (result > max)
                        throw new Emeint.Core.BE.Domain.Exceptions.InvalidOperationException
                            ($"{Key} must be less than {Maximum}",
                            $"{Key} يجب أن تكون أقل من {Maximum}");
                }
            }
            else if (SettingType == SettingType.Decimal)
            {
                bool isDecimal = Decimal.TryParse(value, out decimal result);
                if (!isDecimal)
                    throw new Emeint.Core.BE.Domain.Exceptions.InvalidOperationException
                        ($"{Key} must be a decimal value",
                        $"{Key} يجب أن تكون رقم عشري");

                if (Minimum != null)
                {
                    var min = Convert.ToInt16(Minimum);
                    if (result < min)
                        throw new Emeint.Core.BE.Domain.Exceptions.InvalidOperationException
                            ($"{Key} must be greater than {Minimum}",
                            $"{Key} يجب أن تكون أكبر من {Minimum}");
                }
                if (Maximum != null)
                {
                    var max = Convert.ToInt16(Maximum);
                    if (result > max)
                        throw new Emeint.Core.BE.Domain.Exceptions.InvalidOperationException
                            ($"{Key} must be less than {Maximum}",
                            $"{Key} يجب أن تكون أقل من {Maximum}");
                }
            }
            else if (SettingType == SettingType.Boolean)
            {
                if (value?.Trim().ToLower() != "true" && value?.Trim().ToLower() != "false")
                    throw new Emeint.Core.BE.Domain.Exceptions.InvalidOperationException
                                          ($"{Key} must be true or false",
                                          $"{Key} يجب أن تكون true أو false");
            }
            else if (SettingType == SettingType.Enum)
            {
                if (!string.IsNullOrEmpty(EnumTypeName))
                {
                    var enumType = SettingMapper.GetEnumType(EnumTypeName);

                    if (enumType != null && !Enum.IsDefined(enumType, Convert.ToInt32(value.Trim())))
                    {
                        string enumValues = String.Join(", ", Enum.GetNames(enumType));
                        throw new Emeint.Core.BE.Domain.Exceptions.InvalidOperationException
                            ($"The value of {Key} must belong to [{enumValues}]",
                            $"The value of {Key} must belong to [{enumValues}]");
                    }
                }
            }
            else if (SettingType == SettingType.Email)
            {
                string pattern = @"^\s*[\w\-\+_']+(\.[\w\-\+_']+)*\@[A-Za-z0-9]([\w\.-]*[A-Za-z0-9])?\.[A-Za-z][A-Za-z\.]*[A-Za-z]$";
                var regex = new Regex(pattern);
                if (!regex.IsMatch(value))
                {
                    throw new Emeint.Core.BE.Domain.Exceptions.InvalidOperationException
                        ($"The value of {Key} must be Email pattern",
                        $"The value of {Key} يجب أن تكون بريد الكتروني");
                }
            }
            else if (SettingType == SettingType.URL)
            {
                string pattern = @"^http(s?)\:\/\/[0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*(:(0-9)*)*(\/?)([a-zA-Z0-9\-\.\?\,\'\/\\\+&amp;%\$#_]*)?$";
                var regex = new Regex(pattern);
                if (!regex.IsMatch(value))
                {
                    throw new Emeint.Core.BE.Domain.Exceptions.InvalidOperationException
                        ($"The value of {Key} must be URL pattern",
                        $"The value of {Key} يجب أن تكون URL");
                }
            }
        }
    }
}
