
----------------------------------
For any addition in already developed UDOs
1.From MSVS UDO.b1f file , do the changes and Save UDO to Database.
2.Open same UDO from SAP B1 Studio and Export the file as .xml.
3. Copy the same xml in MSVS solution with UDO_FT_UDONAME.xml.


-----------------------------------
IF UDO.b1f file and UDO.cs file are not coming together under UDO.b1f than follow 
1.Copy the UDO.b1f and UDO.cs file to some other backup folder.
2.Delete both UDO.b1f and USO.cs from solution MSVS.
3.Add new UDO item into solution with same name as UDO was, it will add b1f and cs file.
4.Copy the files from your backup folder and paste it in solution.
5. Now you can see the files together under UDO.b1f.

-----------------Release 1.0

