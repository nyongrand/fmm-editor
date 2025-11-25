using System;
using System.Globalization;
using System.Windows.Data;

namespace FMMEditor.Converters
{
    public class TextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var param = parameter?.ToString();
            switch (param)
            {
                case "EditMode":
                    return Convert_EditMode(value);

                case "RowNumber":
                    return Convert_RowNumber(value);

                case "ScheduleSwap":
                    return Convert_ScheduleSwap(value);

                case "Approved":
                    return Convert_Approved(value);

                default:
                    return null;
            }
        }

        private string Convert_EditMode(object value)
        {
            if (value == null) return null;

            var EditMode = (bool)value;
            if (EditMode)
                return "Simpan Jadwal";
            else
                return "Edit Jadwal";
        }

        private string Convert_RowNumber(object value)
        {
            if (value == null) return null;

            var RowNumber = (int)value;
            return RowNumber + ".";
        }

        private object Convert_ScheduleSwap(object value)
        {
            if (value == null) return null;

            var swap = (int)value;
            switch (swap)
            {
                case 0:
                    return "Belum pernah tukar dinas bulan ini";

                default:
                    return $"{swap} kali tukar dinas bulan ini";
            }
        }

        private object Convert_Approved(object value)
        {
            if (value == null) return null;
            return (bool)value ? "Diterima" : "Ditolak";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
