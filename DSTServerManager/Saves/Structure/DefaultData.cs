using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace DSTServerManager.Saves
{
    class DefaultData
    {
        public DataTable ServersLevelDefaultData()
        {
            DataTable serverLevelData = new DataTable("ServersLevelDefaultData");

            serverLevelData.Columns.Add(new DataColumn("ID", typeof(int)));
            serverLevelData.Columns.Add(new DataColumn("Name", typeof(string)));
            serverLevelData.Columns.Add(new DataColumn("English", typeof(string)));
            serverLevelData.Columns.Add(new DataColumn("Chinese", typeof(string)));
            serverLevelData.Columns.Add(new DataColumn("WorldType", typeof(string)));
            serverLevelData.Columns.Add(new DataColumn("Type", typeof(string)));
            serverLevelData.Columns.Add(new DataColumn("Enum", typeof(string)));
            serverLevelData.Columns.Add(new DataColumn("Current", typeof(string)));

            for (int i = 0; i < 7; i++)
                serverLevelData.Columns[i].ReadOnly = true;

            serverLevelData.Rows.Add(new object[8] { 1, "autumn", "Autumn", "秋天", "Forest", "WorldSeason", "None,Very Short,Short,Default,Long,Very Long,Random", "3" });
            serverLevelData.Rows.Add(new object[8] { 2, "summer", "Summer", "夏天", "Forest", "WorldSeason", "None,Very Short,Short,Default,Long,Very Long,Random", "3" });
            serverLevelData.Rows.Add(new object[8] { 3, "spring", "Spring", "春天", "Forest", "WorldSeason", "None,Very Short,Short,Default,Long,Very Long,Random", "3" });
            serverLevelData.Rows.Add(new object[8] { 4, "winter", "Winter", "冬天", "Forest", "WorldSeason", "None,Very Short,Short,Default,Long,Very Long,Random", "3" });
            serverLevelData.Rows.Add(new object[8] { 5, "berrybush", "Berry Bushes", "浆果丛", "Forest,Caves", "Food", "None,Less,Default,More,Lots", "2" });
            serverLevelData.Rows.Add(new object[8] { 6, "mushroom", "Mushrooms", "蘑菇", "Forest,Caves", "Food", "None,Less,Default,More,Lots", "2" });
            serverLevelData.Rows.Add(new object[8] { 7, "cactus", "", "仙人掌", "Forest", "Food", "None,Less,Default,More,Lots", "2" });
            serverLevelData.Rows.Add(new object[8] { 8, "carrot", "", "胡萝卜", "Forest", "Food", "None,Less,Default,More,Lots", "2" });
            serverLevelData.Rows.Add(new object[8] { 9, "banana", "Cave Bananas", "香蕉", "Caves", "Food", "None,Less,Default,More,Lots", "2" });
            serverLevelData.Rows.Add(new object[8] { 10, "lichen", "Lichen", "苔藓", "Caves", "Food", "None,Less,Default,More,Lots", "2" });

            return serverLevelData;
        }
    }
}
