using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

/* Example Code:
 * 
 * StageDataTable sdt = new StageDataTable("ppap");
 * sdt.LoadCsv("stage.csv");
 * sdt.print();
 * sdt["StageID", 0] = "W1-1";
 * Console.WriteLine(sdt["StageID", 0]);  // output: W1-1
 * sdt["Difficulty", 2] = 99;
 * sdt.SaveCsv("stage1.csv");
 */
public abstract class DataTableBase : DataTable
{
    // NOTE: static abstract는 C# 10부터 지원.
    public abstract IReadOnlyDictionary<string, string> ColumnTypeMapping { get; }

    public DataTableBase(string name) : base(name) { }

    /// <summary>
    /// Get needed CSV files from Resources folder
    /// </summary>
    /// <param name="filename">ex) "table.csv"</param>
    /// <returns></returns>
    public bool LoadCsv(string filename)
    {
        filename = "Assets/Resources/Data/" + filename;
        string[] buffer = System.IO.File.ReadAllLines(filename, System.Text.Encoding.UTF8);

        string[] columns = buffer[0].Split(',');
        foreach (string col in columns)
        {
            DataColumn column = new DataColumn(col);
            column.DataType = System.Type.GetType(ColumnTypeMapping[col]);
            this.Columns.Add(column);
        }

        for (int i = 1; i < buffer.Length; i++)
        {
            if (buffer[i].Trim() == "")  // 빈 줄
                continue;
            string[] items = buffer[i].Split(',')
                .Select(str => str.Trim())
                .ToArray();
            if (items.Length > 0)
                this.Rows.Add(items);  // Automatically does type conversion
        }
        return true;
    }

    /// <summary>
    /// Resources 폴더에 csv 형식으로 파일을 내보낸다.
    /// </summary>
    /// <param name="filename">ex) "table.csv"</param>
    /// <returns></returns>
    public bool SaveCsv(string filename)
    {
        filename = "Assets/Resources/Data/" + filename;

        List<string> buffer = new List<string>();
        string line;

        string[] columnNames = this.Columns
            .Cast<DataColumn>()
            .Select(column => column.ColumnName)
            .ToArray();
        line = string.Join(",", columnNames);
        buffer.Add(line);

        foreach (DataRow row in this.Rows)
        {
            string[] fields = row.ItemArray
                .Select(field => field.ToString())
                .ToArray();
            line = string.Join(",", fields);
            buffer.Add(line);
        }

        System.IO.File.WriteAllLines(filename, buffer.ToArray(), System.Text.Encoding.UTF8);
        return true;
    }

    /// <summary>
    /// 콘솔에 table을 출력한다.
    /// </summary>
    public void print()
    {
        foreach (DataColumn column in this.Columns)
        {
            Console.Write(column.ColumnName);
            Console.Write(":");
            Console.Write(column.DataType);
            Console.Write(" ");
        }
        Console.WriteLine();

        foreach (DataRow row in this.Rows)
        {
            foreach (var item in row.ItemArray)
            {
                Console.Write(item);
                //Console.Write(":");
                //Console.Write(item.GetType());
                Console.Write(" ");
            }
            Console.WriteLine();
        }
    }

    /// <summary>
    /// 주어진 열과 행의 원소를 pandas style로 get 또는 set한다.
    /// </summary>
    /// <param name="col">ex) "Type"</param>
    /// <param name="row">ex) "Elite"</param>
    /// <returns></returns>
    public object this[string col, int row]
    {
        get
        {
            return this.Rows[row][col];
        }
        set
        {
            this.Rows[row][col] = value;
        }
    }
}

public class StageDataTable : DataTableBase
{
    override public IReadOnlyDictionary<string, string> ColumnTypeMapping => _columnTypeMapping;
    private static IReadOnlyDictionary<string, string> _columnTypeMapping = new Dictionary<string, string>()
    {
        { "StageID", "System.String" },
        { "Type", "System.String" },
        { "Difficulty", "System.Int32" },
        { "MonsterID1", "System.String" },
        { "MonsterID2", "System.String" },
        { "MonsterID3", "System.String" },
        { "MonsterID4", "System.String" },
    };

    public StageDataTable(string name) : base(name) { }

    /// <summary>
    /// Type과 Difficulty를 입력하면 해당되는 무작위 DataRow를 리턴한다(사용법은 Dictionary와 비슷).
    /// </summary>
    /// <param name="type"></param>
    /// <param name="difficulty"></param>
    /// <returns>DataRow를 리턴한다. ["StageID"], ["MonsterID1"] 등으로 맵의 ID나 적의 ID를 확인할 수 있다.</returns>
    public DataRow GetRandomEnemyCombination(string type, int difficulty)
    {
        List<DataRow> pool = this.Rows
            .Cast<DataRow>()
            .Where(x => (x["Type"] as string == type) && ((int)x["Difficulty"] == difficulty))
            .ToList();
        pool.Shuffle();
        return pool[0];
    }
}

public class WorldmapRoomDataTable : DataTableBase
{
    override public IReadOnlyDictionary<string, string> ColumnTypeMapping => _columnTypeMapping;
    private static IReadOnlyDictionary<string, string> _columnTypeMapping = new Dictionary<string, string>()
    {
        { "Row", "System.Int32" },  // 위부터 0 ~ BoardSize-1
        { "Column", "System.Int32" },  // 왼쪽부터 0 ~ BoardSize-1
        { "Type", "System.String" },  // None, Monster, Elite, Boss, Shop, Event
        { "Difficulty", "System.Int32" },  // 1 ~ 4
        { "Reward", "System.Int32" }
    };

    public WorldmapRoomDataTable(string name) : base(name) { }
}

public class WorldmapWallDataTable : DataTableBase
{
    override public IReadOnlyDictionary<string, string> ColumnTypeMapping => _columnTypeMapping;
    private static IReadOnlyDictionary<string, string> _columnTypeMapping = new Dictionary<string, string>()
    {
        { "Orientation", "System.String" },  // Horizontal, Vertical
        //{ "Between1", "System.Int32" },  // 어떤 방 번호 사이에 있는지
        //{ "Between2", "System.Int32" },  // ex) 3번방과 7번방 사이의 가로벽이면 Between1=3, Between2=7
        { "Row", "System.Int32" },  // 가로벽일 경우 자기 위의 행(0 ~ BoardSize-2), 세로벽일 경우 자기의 행(0 ~ BoardSize-1)
        { "Column", "System.Int32" },  // 가로벽일 경우 자기의 열(0 ~ BoardSize-1), 세로벽일 경우 자기 왼쪽의 열(0 ~ BoardSize-2)
        { "Type", "System.String" },  // None, Wall, Bush
    };

    public WorldmapWallDataTable(string name) : base(name) { }
}
