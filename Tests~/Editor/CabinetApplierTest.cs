﻿/*
 * Copyright (c) 2023 chocopoi
 * 
 * This file is part of DressingFramework.
 * 
 * DressingFramework is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
 * 
 * DressingFramework is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License along with DressingFramework. If not, see <https://www.gnu.org/licenses/>.
 */

using Chocopoi.DressingFramework.Logging;
using Chocopoi.DressingFramework.Serialization;
using Chocopoi.DressingTools.Api.Cabinet;
using Chocopoi.DressingTools.Api.Wearable;
using NUnit.Framework;

namespace Chocopoi.DressingFramework.Tests
{
    public class CabinetApplierTest : EditorTestBase
    {
        private static void ApplyCabinet(DKReport report, DTCabinet cabinet)
        {
            new CabinetApplier(report, cabinet).RunStages();
        }

        [Test]
        public void AvatarWithOneWearable_AppliesNormally()
        {
            var avatarRoot = InstantiateEditorTestPrefab("DKTest_PhysBoneAvatarWithWearable.prefab");
            var cabinet = avatarRoot.GetComponent<DTCabinet>();
            Assert.NotNull(cabinet);

            var report = new DKReport();
            ApplyCabinet(report, cabinet);

            Assert.False(report.HasLogType(LogType.Error), "Should have no errors");
        }

        // TODO: new test for version

        // [Test]
        // public void ConfigVersionTooNew_ReturnsCorrectErrorCodes()
        // {
        //     var avatarRoot = InstantiateRuntimeTestPrefab("DKTest_PhysBoneAvatarWithWearable.prefab");
        //     var cabinet = avatarRoot.GetComponent<DTCabinet>();

        //     // we simulate this by adding the config version by one
        //     var wearableComp = avatarRoot.GetComponentInChildren<DTWearable>();
        //     Assert.NotNull(wearableComp);
        //     JObject json = JObject.Parse(wearableComp.configJson);
        //     json["configVersion"] = WearableConfig.CurrentConfigVersion + 1;
        //     wearableComp.configJson = json.ToString(Formatting.None);

        //     var report = new DTReport();
        //     ApplyCabinet(report, cabinet);

        //     Assert.True(report.HasLogCode(CabinetApplier.MessageCode.IncompatibleConfigVersion), "Should have incompatible config version error");
        // }

        // TODO: write config migration test

        [Test]
        public void WearableConfigDeserializationFailure_ReturnsCorrectErrorCodes()
        {
            var avatarRoot = InstantiateEditorTestPrefab("DKTest_PhysBoneAvatarWithWearable.prefab");
            var cabinet = avatarRoot.GetComponent<DTCabinet>();
            Assert.NotNull(cabinet);

            // we simulate this by destroying the config json
            var wearableComp = avatarRoot.GetComponentInChildren<DTWearable>();
            Assert.NotNull(wearableComp);
            wearableComp.configJson = "ababababababababa";

            var report = new DKReport();
            ApplyCabinet(report, cabinet);

            Assert.True(report.HasLogCode(CabinetApplier.MessageCode.UnableToDeserializeWearableConfig), "Should have deserialization error");
        }

        [Test]
        public void GroupDynamicsToSeparateGameObjectsCorrectly()
        {
            var avatarRoot = InstantiateEditorTestPrefab("DKTest_PhysBoneAvatarWithWearableOtherDynamics.prefab");
            var cabinet = avatarRoot.GetComponent<DTCabinet>();
            Assert.NotNull(cabinet);
            Assert.True(CabinetConfigUtility.TryDeserialize(cabinet.configJson, out var cabinetConfig));

            var report = new DKReport();
            cabinetConfig.groupDynamics = true;
            cabinetConfig.groupDynamicsSeparateGameObjects = true;
            ApplyCabinet(report, cabinet);

            Assert.False(report.HasLogType(LogType.Error), "Should have no errors");

            // get wearable root
            var wearableRoot = avatarRoot.transform.Find("DKTest_PhysBoneWearable");
            Assert.NotNull(wearableRoot);

            // get dynamics container
            var dynamicsContainer = wearableRoot.Find("DT_Dynamics");
            Assert.NotNull(dynamicsContainer);

            // check dynamics
            var wearableDynamicsList = DKEditorUtils.ScanDynamics(wearableRoot.gameObject);
            foreach (var wearableDynamics in wearableDynamicsList)
            {
                Assert.AreEqual(dynamicsContainer, wearableDynamics.Transform.parent);
            }
        }

        [Test]
        public void GroupDynamicsToSingleGameObjectCorrectly()
        {
            var avatarRoot = InstantiateEditorTestPrefab("DKTest_PhysBoneAvatarWithWearableOtherDynamics.prefab");
            var cabinet = avatarRoot.GetComponent<DTCabinet>();
            Assert.NotNull(cabinet);
            Assert.True(CabinetConfigUtility.TryDeserialize(cabinet.configJson, out var cabinetConfig));

            var report = new DKReport();
            cabinetConfig.groupDynamics = true;
            cabinetConfig.groupDynamicsSeparateGameObjects = false;
            cabinet.configJson = CabinetConfigUtility.Serialize(cabinetConfig);
            ApplyCabinet(report, cabinet);

            Assert.False(report.HasLogType(LogType.Error), "Should have no errors");

            // get wearable root
            var wearableRoot = avatarRoot.transform.Find("DKTest_PhysBoneWearable");
            Assert.NotNull(wearableRoot);

            // get dynamics container
            var dynamicsContainer = wearableRoot.Find("DT_Dynamics");
            Assert.NotNull(dynamicsContainer);

            // check dynamics
            var wearableDynamicsList = DKEditorUtils.ScanDynamics(wearableRoot.gameObject);
            foreach (var wearableDynamics in wearableDynamicsList)
            {
                Assert.AreEqual(dynamicsContainer, wearableDynamics.Transform);
            }
        }
    }
}
