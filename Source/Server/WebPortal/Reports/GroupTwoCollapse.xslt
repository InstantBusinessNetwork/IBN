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
		<table class="SectionTable"
				style="margin-top:2px;table-layout:fixed; BORDER-RIGHT: #c4c4b4 1px; BORDER-TOP: #c4c4b4 1px; BORDER-LEFT: #c4c4b4 1px; WIDTH: 100%; BORDER-BOTTOM: #c4c4b4 1px; BORDER-COLLAPSE: collapse; BACKGROUND-COLOR: white"
				borderColor="#c4c4b4" cellspacing="0" cellpadding="0"
				rules="all" border="1" width="100%">
			<tr class="SectionTableHeader" style="COLOR: black; BACKGROUND-COLOR: #EDE9E0">
				<td style="Padding:3px;">
					<xsl:attribute name="width">
						<xsl:value-of select="100 div count(//Report/Headers/node())" />%
					</xsl:attribute>
					<strong>
						<xsl:value-of select="//Report/Headers/Header[1]/@Description" />
					</strong>
				</td>
				<td>
					<xsl:attribute name="colspan">
						<xsl:value-of select="count(//Report/Headers/node())-1" />
					</xsl:attribute>
					<table style="table-layout:fixed" width="100%"  cellspacing="0" cellpadding="0" >
						<tr>
							<td style="Padding:3px;BORDER-BOTTOM: #c4c4b4 1px solid;BORDER-Right: #c4c4b4 1px solid;">
								<xsl:attribute name="width">
									<xsl:value-of select="(100 div (count(//Report/Headers/node())-1))" />%
								</xsl:attribute>
								<strong>
									<xsl:value-of select="//Report/Headers/Header[2]/@Description" />
								</strong>
							</td>
							<td>
								<xsl:attribute name="colspan">
									<xsl:value-of select="count(//Report/Headers/node())-2" />
								</xsl:attribute>
								<table style="table-layout:fixed;"  width="100%" cellspacing="0" cellpadding="0" >
									<tr>
										<xsl:for-each select="//Report/Headers/node()">
											<xsl:if test="position()>2">
												<td style="Padding:3px;BORDER-BOTTOM: #c4c4b4 1px solid;BORDER-Right: #c4c4b4 1px solid;">
													<xsl:attribute name="width">
														<xsl:value-of select="100 div (count(//Report/Headers/node())-2)" />%
													</xsl:attribute>
													<strong>
														<xsl:value-of select="./@Description" />
													</strong>
												</td>
											</xsl:if>
										</xsl:for-each>
									</tr>
								</table>
							</td>
						</tr>
					</table>
				</td>
			</tr>

			<xsl:for-each select="//Report/Groups/Group">
				<tr>
					<td style="BORDER-BOTTOM: #c4c4b4 2px solid;Padding:3px" valign="top" align="left" bgcolor="#e5e8eb">
						<span onclick="collapse_expand(this);" style="cursor:pointer;">
							<img src='../layouts/images/minus.gif' border='0' />
						</span>
						<img src='../layouts/images/spacer.gif' width='5px' align='middle' border='0' />
						<font color="#000000">
							<strong>
								<xsl:value-of select="./@Name" />
							</strong>
						</font>
					</td>
					<td valign="top" align="left" bgcolor="#e5e8eb">
						<xsl:attribute name="colspan">
							<xsl:value-of select="count(//Report/Headers/node())-1" />
						</xsl:attribute>
						<table style="table-layout:fixed" width="100%"  cellspacing="0" cellpadding="0" >
							<xsl:for-each select="./Groups/Group">
								<tr>
									<td style="Padding:3px;BORDER-BOTTOM: #c4c4b4 1px solid;BORDER-Right: #c4c4b4 1px solid;" valign="top" align="left" bgcolor="#e5e8eb">
										<xsl:attribute name="width">
											<xsl:value-of select="(100 div (count(//Report/Headers/node())-1))" />%
										</xsl:attribute>
										<span onclick="collapse_expand(this);" style="cursor:pointer;">
											<img src='../layouts/images/minus.gif' border='0' />
										</span>
										<img src='../layouts/images/spacer.gif' width='5px' align='middle' border='0' />
										<font color="#000000">
											<strong>
												<xsl:value-of select="./@Name" />
											</strong>
										</font>
									</td>
									<td valign="top" align="left">
										<xsl:attribute name="colspan">
											<xsl:value-of select="count(//Report/Headers/node())-2" />
										</xsl:attribute>
										<table style="table-layout:fixed;"  width="100%" cellspacing="0" cellpadding="0" >
											<xsl:choose>
												<xsl:when test="count(./Item)=0">
													<tr>
														<xsl:for-each select="//Report/Headers/node()">
															<xsl:if test="position()>2">
																<td bgcolor="#e5e8eb" style="Padding:3px;BORDER-BOTTOM: #c4c4b4 1px solid;BORDER-Right: #c4c4b4 1px solid;">
																	<font color="#e5e8eb">-</font>
																</td>
															</xsl:if>
														</xsl:for-each>
													</tr>
												</xsl:when>
												<xsl:otherwise>
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
																<td style="Padding:3px;BORDER-BOTTOM: #c4c4b4 1px solid;BORDER-Right: #c4c4b4 1px solid;" valign="top">
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
																					<div>
																						<xsl:value-of select="vb:GetCDate(Values/Value)" />
																					</div>
																				</xsl:when>
																				<xsl:when test="./@DataType[.='Time']">
																					<div>
																						<xsl:value-of select="vb:GetCTime(Values/Value)" />
																					</div>
																				</xsl:when>
																				<xsl:otherwise>
																					<div>
																						<xsl:value-of select="Values/Value" />
																					</div>
																				</xsl:otherwise>
																			</xsl:choose>
																		</xsl:otherwise>
																	</xsl:choose>
																</td>
															</xsl:for-each>
														</tr>
													</xsl:for-each>
												</xsl:otherwise>
											</xsl:choose>
										</table>
									</td>
								</tr>
							</xsl:for-each>
						</table>
					</td>
				</tr>
			</xsl:for-each>
		</table>

	</xsl:template>

</xsl:stylesheet>
