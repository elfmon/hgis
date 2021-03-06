﻿
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Xml;

using System.Collections.Generic;
using System.Text;



namespace HGIS.GDAL
{
    /// <summary>
    /// WMS driver based on GDAL api.
    /// This class is meant to provide a WMS interface for GDAL raster formats - specifically for jp2 and ecw 
    /// 
    /// Note:
    /// comparing to jp2 ecw is lightning fast, though its licensing is pretty restrictive, so ecw should NOT be used as soon as this software is used commercially
    /// 
    /// Note:
    /// this class is not meant to provide fully featured wms operations stack, but rather aims at being quick in raster data extraction and processing.
    /// for example reprojections are not handled at all - use the data that does not require cs adjustments!
    /// </summary>
    public partial class WmsDriver : Cartomatic.Wms.WmsDriver.Base, IDisposable
    {
        /// <summary>
        /// creates a new driver instance
        /// </summary>
        /// <param name="gdalPath">path to the gdal stuff</param>
        /// <param name="dataSource">data source file path</param>
        /// <param name="layerName">Optional layer name to be used in the LAYERS param of the GetMap request instead of the file name</param>
        /// <param name="srid">srid of the data source</param>
        /// <param name="serviceDescription">service description object</param>
        public WmsDriver(
            string gdalPath,
            string dataSource,
            string layerName,
            string srid,
            Cartomatic.Wms.WmsDriver.WmsServiceDescription serviceDescription
        )
        {
            //wms service description
            this.ServiceDescription = serviceDescription;

            GdalPath = gdalPath;

            //well if this throws errors, then the param is not valid ayway ;) kinda forcing to check this prior to instantiating this object
            //the base wms driver tests the epsg in order to properly understand the bbox and it expets int hence parsing
            this.SRID = Int32.Parse(srid); 
            
            this.DataSource = dataSource;
            this.DataSourceName = layerName;

            ApplyWmsSettings();
        }

        private void ApplyWmsSettings()
        {
            this.SupportedVersions.Add("1.3.0");

            this.SupportedGetCapabilitiesFormats.Add("text/xml");

            this.SupportedGetMapFormats.Add("image/png");
            this.SupportedGetMapFormats.Add("image/jpeg");
            this.SupportedGetMapFormats.Add("image/gif");

            this.SupportedExceptionFormats.Add("XML");
            

            //supported info formats
            //so far no info for this driver; not likely to be implemented at all...

        }


        /// <summary>
        /// cleanup, cleanup, everybody cleanup
        /// </summary>
        public void Dispose()
        {
            if (GdalDataset != null)
            {
                GdalDataset.Dispose();
            }
            GdalDataset = null;
        }
    }
}
