<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="/">
		<table cellspacing="4" cellpadding="4" width="100%">
			<xsl:for-each select="root">
			<tr>
				<td class="text" width="140"><strong>Group Type:</strong></td>
				<td class="text"><xsl:value-of select="GroupType"/></td>
			</tr>
			<tr>
				<td class="text" width="140"><strong>Group Size:</strong></td>
				<td class="text"><xsl:value-of select="GroupSize"/></td>
			</tr>
			<tr>
				<td class="text" width="140"><strong>Industry:</strong></td>
				<td class="text"><xsl:value-of select="Industry"/></td>
			</tr>
			<tr>
				<td class="text" width="140"><strong>Reference:</strong></td>
				<td class="text"><xsl:value-of select="Reference"/></td>
			</tr>
			<tr>
				<td class="text" width="140"><strong>ZIP:</strong></td>
				<td class="text"><xsl:value-of select="ZIP"/></td>
			</tr>
			<tr>
				<td class="text" width="140"><strong>Country:</strong></td>
				<td class="text"><xsl:value-of select="Country"/></td>
			</tr>
			</xsl:for-each>
		</table>
	</xsl:template>
</xsl:stylesheet>

  