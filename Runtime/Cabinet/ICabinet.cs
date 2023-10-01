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

using UnityEngine;

namespace Chocopoi.DressingFramework.Cabinet
{
    /// <summary>
    /// Cabinet interface
    /// </summary>
    public interface ICabinet
    {
        /// <summary>
        /// Avatar GameObject. If rootGameObject is `null`, this will return the GameObject holding this component.
        /// </summary>
        GameObject AvatarGameObject { get; set; }

        /// <summary>
        /// Serialized cabinet config JSON. Use `CabinetConfigUtility.Deserialize()` to deserialize.
        /// </summary>
        string ConfigJson { get; set; }
    }
}
