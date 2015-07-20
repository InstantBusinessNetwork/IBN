<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:msxsl="urn:schemas-microsoft-com:xslt"
	xmlns:vb="http://www.mediachase.ru/vb"
	xmlns:js="http://www.mediachase.ru/js"
	xmlns:dt="urn:schemas-microsoft-com:datatypes"
	version="1.0">
	<msxsl:script language="C#" implements-prefix="vb">
		public string GetCDate(XPathNodeIterator who)
		{
		who.MoveNext();
		if(who.Count &gt; 0)
		foreach(object _w in who)
		{
		string s= _w.ToString();
		//if(s.Length &gt; 9)
		//s = s.Substring(0,10);
		DateTime time = DateTime.Parse(s);
		return time.ToString("d");
		}
		return "";
		}

		public string GetCTime(XPathNodeIterator who)
		{
		who.MoveNext();
		if(who.Count &gt; 0)
		foreach(object _w in who)
		{
		int intMin= int.Parse(_w.ToString());
		int iMinutes = 0;
		int iHours = Math.DivRem(intMin, 60, out iMinutes);
		string sMinutes = (iMinutes &lt; 10) ? ("0" + iMinutes.ToString()) : iMinutes.ToString();
		return iHours.ToString() + ":" + sMinutes;
		}
		return "";
		}
	</msxsl:script>
	<xsl:template match="/">

		<table>
			<tr>
				<td height="15px"></td>
			</tr>
		</table>
		<xsl:for-each select="//Report/Groups/Group">
			<table>
				<tr>
					<td height="7px"></td>
				</tr>
			</table>

			<div class="ResourceSubhead">
				<B>
					<xsl:value-of select="./@Name" />
				</B>
			</div>
			<table class="SectionTable"
					style="margin-top:2px;BORDER-RIGHT: #c4c4b4 1px; BORDER-TOP: #c4c4b4 1px; BORDER-LEFT: #c4c4b4 1px; WIDTH: 100%; BORDER-BOTTOM: #c4c4b4 1px; BORDER-COLLAPSE: collapse; BACKGROUND-COLOR: white"
					borderColor="#c4c4b4" cellspacing="0" cellpadding="3"
					rules="all" border="1" width="100%">
				<tr class="SectionTableHeader" style="COLOR: black; BACKGROUND-COLOR: #EDE9E0">
					<xsl:for-each select="./Item[1]/node()">
						<td>
							<strong>
								<xsl:value-of select="./@Description" />
							</strong>
						</td>
					</xsl:for-each>
				</tr>
				<xsl:for-each select="./Item">
					<xsl:sort select="Field[@Name=//Report/IBNReportTemplate/Sorting/Sort[1][@Asc='1' and @DataType!='Int32']/@FieldName]/Values/Value" order="ascending" data-type="text"></xsl:sort>
					<xsl:sort select="Field[@Name=//Report/IBNReportTemplate/Sorting/Sort[1][@Asc='1' and @DataType='Int32']/@FieldName]/Values/Value" order="ascending" data-type="number"></xsl:sort>
					<xsl:sort select="Field[@Name=//Report/IBNReportTemplate/Sorting/Sort[1][@Asc='0' and @DataType!='Int32']/@FieldName]/Values/Value" order="descending" data-type="text"></xsl:sort>
					<xsl:sort select="Field[@Name=//Report/IBNReportTemplate/Sorting/Sort[1][@Asc='0' and @DataType='Int32']/@FieldName]/Values/Value" order="descending" data-type="number"></xsl:sort>
					<xsl:sort select="Field[@Name=//Report/IBNReportTemplate/Sorting/Sort[2][@Asc='1' and @DataType!='Int32']/@FieldName]/Values/Value" order="ascending" data-type="text"></xsl:sort>
					<xsl:sort select="Field[@Name=//Report/IBNReportTemplate/Sorting/Sort[2][@Asc='1' and @DataType='Int32']/@FieldName]/Values/Value" order="ascending" data-type="number"></xsl:sort>
					<xsl:sort select="Field[@Name=//Report/IBNReportTemplate/Sorting/Sort[2][@Asc='0' and @DataType!='Int32']/@FieldName]/Values/Value" order="descending" data-type="text"></xsl:sort>
					<xsl:sort select="Field[@Name=//Report/IBNReportTemplate/Sorting/Sort[2][@Asc='0' and @DataType='Int32']/@FieldName]/Values/Value" order="descending" data-type="number"></xsl:sort>
					<xsl:sort select="Field[@Name=//Report/IBNReportTemplate/Sorting/Sort[3][@Asc='1' and @DataType!='Int32']/@FieldName]/Values/Value" order="ascending" data-type="text"></xsl:sort>
					<xsl:sort select="Field[@Name=//Report/IBNReportTemplate/Sorting/Sort[3][@Asc='1' and @DataType='Int32']/@FieldName]/Values/Value" order="ascending" data-type="number"></xsl:sort>
					<xsl:sort select="Field[@Name=//Report/IBNReportTemplate/Sorting/Sort[3][@Asc='0' and @DataType!='Int32']/@FieldName]/Values/Value" order="descending" data-type="text"></xsl:sort>
					<xsl:sort select="Field[@Name=//Report/IBNReportTemplate/Sorting/Sort[3][@Asc='0' and @DataType='Int32']/@FieldName]/Values/Value" order="descending" data-type="number"></xsl:sort>
					<xsl:sort select="Field[@Name=//Report/IBNReportTemplate/Sorting/Sort[4][@Asc='1' and @DataType!='Int32']/@FieldName]/Values/Value" order="ascending" data-type="text"></xsl:sort>
					<xsl:sort select="Field[@Name=//Report/IBNReportTemplate/Sorting/Sort[4][@Asc='1' and @DataType='Int32']/@FieldName]/Values/Value" order="ascending" data-type="number"></xsl:sort>
					<xsl:sort select="Field[@Name=//Report/IBNReportTemplate/Sorting/Sort[4][@Asc='0' and @DataType!='Int32']/@FieldName]/Values/Value" order="descending" data-type="text"></xsl:sort>
					<xsl:sort select="Field[@Name=//Report/IBNReportTemplate/Sorting/Sort[4][@Asc='0' and @DataType='Int32']/@FieldName]/Values/Value" order="descending" data-type="number"></xsl:sort>
					<xsl:sort select="Field[@Name=//Report/IBNReportTemplate/Sorting/Sort[5][@Asc='1' and @DataType!='Int32']/@FieldName]/Values/Value" order="ascending" data-type="text"></xsl:sort>
					<xsl:sort select="Field[@Name=//Report/IBNReportTemplate/Sorting/Sort[5][@Asc='1' and @DataType='Int32']/@FieldName]/Values/Value" order="ascending" data-type="number"></xsl:sort>
					<xsl:sort select="Field[@Name=//Report/IBNReportTemplate/Sorting/Sort[5][@Asc='0' and @DataType!='Int32']/@FieldName]/Values/Value" order="descending" data-type="text"></xsl:sort>
					<xsl:sort select="Field[@Name=//Report/IBNReportTemplate/Sorting/Sort[5][@Asc='0' and @DataType='Int32']/@FieldName]/Values/Value" order="descending" data-type="number"></xsl:sort>

					<tr>
						<xsl:choose>
							<xsl:when test="position() mod 2">
								<xsl:attribute name="class">SectionTableRow</xsl:attribute>
							</xsl:when>
							<xsl:otherwise>
								<xsl:attribute name="class">SectionTableRowAlt</xsl:attribute>
							</xsl:otherwise>
						</xsl:choose>
						<xsl:for-each select="node()">
							<td valign="top">
								<xsl:choose>
									<xsl:when test="./@Type[.='3' or .='2']">
										<xsl:choose>
											<xsl:when test="./@DataType[.='DateTime']">
												<xsl:for-each select="Values/Value">
													<div style="padding:2px;">
														<xsl:value-of select="vb:GetCDate(.)" />
													</div>
												</xsl:for-each>
											</xsl:when>
											<xsl:when test="./@DataType[.='Time']">
												<xsl:for-each select="Values/Value">
													<div style="padding:2px;">
														<xsl:value-of select="vb:GetCTime(.)" />
													</div>
												</xsl:for-each>
											</xsl:when>
											<xsl:otherwise>
												<xsl:for-each select="Values/Value">
													<div style="padding:2px;">
														<xsl:value-of select="." />
													</div>
												</xsl:for-each>
											</xsl:otherwise>
										</xsl:choose>
									</xsl:when>
									<xsl:otherwise>
										<xsl:choose>
											<xsl:when test="./@DataType[.='DateTime']">
												<xsl:value-of select="vb:GetCDate(Values/Value)" />
											</xsl:when>
											<xsl:when test="./@DataType[.='Time']">
												<xsl:value-of select="vb:GetCTime(Values/Value)" />
											</xsl:when>
											<xsl:otherwise>
												<xsl:value-of select="Values/Value" />
											</xsl:otherwise>
										</xsl:choose>
									</xsl:otherwise>
								</xsl:choose>
							</td>
						</xsl:for-each>
					</tr>
				</xsl:for-each>
			</table>

		</xsl:for-each>
	</xsl:template>
</xsl:stylesheet>
