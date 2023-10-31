using System;
using System.IO;
using Newtonsoft.Json;
using System.Xml.Serialization;
using System.Xml;

// Модель данных для сериализации и десериализации
[Serializable]
public class DataModel
{
    public string Property1 { get; set; }
    public string Property2 { get; set; }
}

// Класс для чтения и записи файла
public class FileManager
{
    private string filePath;

    public FileManager(string path)
    {
        filePath = path;
    }

    public DataModel LoadData()
    {
        DataModel data = null;

        if (File.Exists(filePath))
        {
            string fileExtension = Path.GetExtension(filePath).ToLower();

            if (fileExtension == ".json")
            {
                string json = File.ReadAllText(filePath);
                data = JsonConvert.DeserializeObject<DataModel>(json);
            }
            else if (fileExtension == ".xml")
            {
                XmlSerializer serializer = new XmlSerializer(typeof(DataModel));
                using (FileStream stream = new FileStream(filePath, FileMode.Open))
                {
                    data = (DataModel)serializer.Deserialize(stream);
                }
            }
            else if (fileExtension == ".txt")
            {
                string text = File.ReadAllText(filePath);
                data = new DataModel { Property1 = text };
            }
            else
            {
                Console.WriteLine("Unsupported file format.");
            }
        }

        return data;
    }

    public void SaveData(DataModel data)
    {
        Console.WriteLine("Выберите формат для сохранения: (1 - JSON, 2 - XML, 3 - TXT)");
        var formatChoice = Console.ReadLine();

        string fileExtension = "";

        switch (formatChoice)
        {
            case "1":
                fileExtension = ".json";
                break;
            case "2":
                fileExtension = ".xml";
                break;
            case "3":
                fileExtension = ".txt";
                break;
            default:
                Console.WriteLine("Неверный выбор формата.");
                return;
        }

        Console.Write("Введите путь для сохранения файла: ");
        var savePath = Console.ReadLine();

        string newFilePath = Path.Combine(savePath, Path.GetFileNameWithoutExtension(filePath) + fileExtension);

        if (fileExtension == ".json")
        {
            string json = JsonConvert.SerializeObject(data, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(newFilePath, json);
        }
        else if (fileExtension == ".xml")
        {
            XmlSerializer serializer = new XmlSerializer(typeof(DataModel));
            using (FileStream stream = new FileStream(newFilePath, FileMode.Create))
            {
                serializer.Serialize(stream, data);
            }
        }
        else if (fileExtension == ".txt")
        {
            File.WriteAllText(newFilePath, data.Property1);
        }

        Console.WriteLine($"Данные сохранены в файле: {newFilePath}");
    }
}

// Класс для изменения текста
public class TextEditor
{
    private DataModel data;

    public TextEditor(DataModel data)
    {
        this.data = data;
    }

    public void EditText()
    {
        // Здесь вы можете добавить логику для изменения текста
        // Например, можно реализовать меню для редактирования свойств объекта data
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.Write("Введите путь к файлу: ");
        string filePath = Console.ReadLine();

        FileManager fileManager = new FileManager(filePath);
        DataModel data = fileManager.LoadData();

        if (data != null)
        {
            Console.WriteLine("Содержимое файла:");
            Console.WriteLine(data.Property1); // Выводим содержимое на экран

            TextEditor textEditor = new TextEditor(data);
            textEditor.EditText();

            Console.WriteLine("Нажмите F1, чтобы сохранить данные, или Escape, чтобы выйти.");

            while (true)
            {
                var key = Console.ReadKey();
                if (key.Key == ConsoleKey.F1)
                {
                    fileManager.SaveData(data);
                }
                else if (key.Key == ConsoleKey.Escape)
                {
                    break;
                }
            }
        }
        else
        {
            Console.WriteLine("Файл не найден или не поддерживаемый формат.");
        }
    }
}
