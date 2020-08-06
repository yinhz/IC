using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IC.Core.Test
{
    [TestClass]
    public class VirtualLotTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            // 设备反馈上料

            // 01 记录到 resource_loading 表
            // 02 形成虚拟批次（ Copy resource_loading 记录过去即可，形成新的 virtual_lot )
            //      假设最后的虚拟批次是 V1
            var EquipmentInfo_InMemory = new
            {
                EquipmetCode = "EQU001",
                ConsumeVirtualLot = "V1",
                ConsumeVirtualLotContent = new object[2] {
                  new {
                    Container = "C1",
                    ProductNo = "P1",
                    LotNo = "L1",
                    Qty = 100
                  },
                  new {
                    Container = "C2",
                    ProductNo = "P2",
                    LotNo = "L2",
                    Qty = 200
                  }
                },
                GenealogyVirtualLot = ""
            };

            // 电芯出站
            // wip_his 中记录 serialno + virtual_lot 关系

            // 出站同时反馈了消耗信息
            // 比较 设备反馈的消耗
            var cell_outbound_consumed = new object();

            if (cell_outbound_consumed == EquipmentInfo_InMemory.ConsumeVirtualLotContent)
            {
                if (EquipmentInfo_InMemory.GenealogyVirtualLot == "")
                {
                    // 记录 
                }
                EquipmentInfo_InMemory.GenealogyVirtualLot = EquipmentInfo_InMemory.ConsumeVirtualLot;
            }

            // 反馈下料信息
        }
    }
}
