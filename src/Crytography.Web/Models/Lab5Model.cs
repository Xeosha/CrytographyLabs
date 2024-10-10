namespace Crytography.Web.Models
{
    public class Lab5Model
    {
        public string InputCode { get; set; } = string.Empty; // Введённая пользователем 8-битная последовательность
        public string EditableCode { get; set; } = string.Empty; // Изменяемая часть с 8 битами
        public int ParityBit { get; set; } // Бит четности (отдельно)
        public int OldParityBit { get; set; } // Старый бит четности
        public bool HasError { get; set; } // Флаг ошибки
        public bool CheckPerformed { get; set; } // Проверка была выполнена
    }
}
