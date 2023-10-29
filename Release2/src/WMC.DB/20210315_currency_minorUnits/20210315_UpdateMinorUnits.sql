--
Update [currency]  
Set [MinorUnits] = 0
where [code] in (
 'BIF'  --0 
,'CLP'	--0
,'DJF'	--0
,'GNF'	--0
,'JPY'	--0
,'KMF'	--0
,'KRW'	--0
,'MGA'	--0
,'PYG'	--0
,'RWF'	--0
,'UGX'	--0
,'VND'	--0
,'VUV'	--0
,'XAF'	--0
,'XOF'	--0
,'XPF'  --0
,'ISK'  --0
)

Update [currency]  
Set [MinorUnits] = 3
where [code] in (
'IQD'	--3
,'BHD'  --3
,'JOD'	--3
,'KWD'	--3
,'LYD'	--3
,'OMR'	--3
,'TND'  --3
)

