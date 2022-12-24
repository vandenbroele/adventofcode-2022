<Query Kind="Program">
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>Newtonsoft.Json.Linq</Namespace>
  <Namespace>Newtonsoft.Json.Serialization</Namespace>
</Query>

void Main()
{
	Run(sampleInput).Dump("sample");
	Run(puzzleInput).Dump("puzzle");
}

public object Run(string input)
{
	var ops = IOp.ParseInput(input);
	var root = ops["root"] as CalcOp;
	
	//((ValueOp)ops["humn"]).Value = 3952288690726;
	//
	//root.GetLeftValue(ops).Dump();
	//root.GetRightValue(ops).Dump();
	
	return new
	{
		Part1 = root.GetValue(ops),
		Part2 = root.FindValueFor("humn", ops),
	};
}

public interface IOp
{
	string Name { get; set; }
	double GetValue(IDictionary<string, IOp> ops);
	public bool Contains(string name, IDictionary<string, IOp> ops);
	public double FindValueFor(string name, double expected, IDictionary<string, IOp> ops) => expected;

	public static IOp Parse(string value)
	{
		var valueOp = Regex.Match(value, @"(?<name>\w+): (?<value>\d+)");
		if (valueOp.Success) return new ValueOp
		{
			Name = valueOp.Groups["name"].Value,
			Value = double.Parse(valueOp.Groups["value"].Value)
		};

		var calcOp = Regex.Match(value, @"(?<name>\w+): (?<a>\w+) (?<operator>.) (?<b>\w+)");
		if (calcOp.Success) return new CalcOp
		{
			Name = calcOp.Groups["name"].Value,
			Left = calcOp.Groups["a"].Value,
			Operator = calcOp.Groups["operator"].Value,
			Right = calcOp.Groups["b"].Value,
		};

		throw new ArgumentOutOfRangeException();
	}

	public static IDictionary<string, IOp> ParseInput(string input)
		=> input.Split("\r\n").Select(Parse).ToDictionary(k => k.Name);
}

public class ValueOp : IOp
{
	public string Name { get; set; }
	public double Value { get; set; }

	public bool Contains(string name, IDictionary<string, IOp> ops) => Name == name;
	public double GetValue(IDictionary<string, IOp> ops) => Value;
}
public class CalcOp : IOp
{
	public string Name { get; set; }

	public string Left { get; set; }
	public string Operator { get; set; }
	public string Right { get; set; }

	public bool Contains(string name, IDictionary<string, IOp> ops)
		=> Name == name || ops[Left].Contains(name, ops) || ops[Right].Contains(name, ops);

	public double GetLeftValue(IDictionary<string, IOp> ops) => ops[Left].GetValue(ops);
	public double GetRightValue(IDictionary<string, IOp> ops) => ops[Right].GetValue(ops);

	public double GetValue(IDictionary<string, IOp> ops)
	{
		var a = GetLeftValue(ops);
		var b = GetRightValue(ops);

		return Operator switch
		{
			"+" => a + b,
			"-" => a - b,
			"*" => a * b,
			"/" => a / b,
			_ => throw new InvalidOperationException(Operator),
		};
	}


	public double FindValueFor(string name, IDictionary<string, IOp> ops)
	{
		var left = ops[Left];
		var right = ops[Right];

		if (left.Contains(name, ops))
			return left.FindValueFor(name, right.GetValue(ops), ops);
			
		if (right.Contains(name, ops))
			return right.FindValueFor(name, left.GetValue(ops), ops);

		throw new Exception();
	}

	public double FindValueFor(string name, double expected, IDictionary<string, IOp> ops)
	{
		var left = ops[Left];
		var right = ops[Right];

		if (left.Contains(name, ops))
		{
			var rightValue = right.GetValue(ops);
			var leftExpected = Operator switch
			{
				"+" => expected - rightValue, // leftExpected + rightValue = expected
				"-" => expected + rightValue, // leftExpected - rightValue = expected
				"*" => expected / rightValue, // leftExpected * rightValue = expected
				"/" => expected * rightValue, // leftExpected / rightValue = expected
			};
			return left.FindValueFor(name, leftExpected, ops);
		}

		if (right.Contains(name, ops))
		{
			var leftValue = left.GetValue(ops);
			var rightExpected = Operator switch
			{
				"+" => expected - leftValue, // leftValue + rightExpected = expected
				"-" => leftValue - expected, // leftValue - rightExpected = expected
				"*" => expected / leftValue, // leftValue * rightExpected = expected
				"/" => leftValue / expected, // leftValue / rightExpected = expected
			};

			return right.FindValueFor(name, rightExpected, ops);
		}

		throw new Exception();
	}
}

public static string sampleInput = @"root: pppw + sjmn
dbpl: 5
cczh: sllz + lgvd
zczc: 2
ptdq: humn - dvpt
dvpt: 3
lfqf: 4
humn: 5
ljgn: 2
sjmn: drzm * dbpl
sllz: 4
pppw: cczh / lfqf
lgvd: ljgn * ptdq
drzm: hmdt - zczc
hmdt: 32";

public static string puzzleInput = @"smwb: jtdc - grsn
rcjj: 2
ndwc: 2
ltrg: 3
jpvm: 13
pgtv: 3
mttv: 4
lwmg: 3
zgtn: zwbm / hjzc
swrw: 5
qjzt: 3
dmjp: vzlr + hlts
jmhz: 8
cjqt: drzh * zzcf
cmml: zmqb * zcwt
qwwn: 2
rsvf: 11
qjfn: 8
cgfg: ljfh / blbg
dltj: ctmq * ppzl
tdjq: 3
hlts: 3
cbmd: mmsh + rbbl
lqqg: zzcv * lcjw
vlcf: 11
zzwp: 5
hrvm: 3
blgq: 4
zmqb: wtgq + wgst
dbtg: 4
bhrg: 4
chtc: czww + rmhv
ncrn: 17
lctl: 2
pcvs: 8
nhhz: svgg + jfqv
zvjm: 3
svbm: qwgh / zwdn
ghjf: rfdb * qvrn
hqzc: 19
zdzg: vvrr * fmjv
lgcg: 3
fdpd: 1
vhtz: wgms * qfrj
zhvf: svch * gngb
rfnf: gvhs + pwlg
hccq: 2
rlsn: jdmf + lgwv
wzdg: twjf + mbcj
sfvl: 2
tjpb: 16
jcrl: snph + jwfw
pbrv: jwtt + tvpv
ghdj: lzzr + bqcn
lfss: 2
lpbf: 5
fmrt: bfhg + scgt
svpb: qdcg * hmfw
hsgw: 17
bvnb: 13
bggm: 8
wjnq: msfw * dcqt
wfgv: dhwh * pnpt
vvdl: bqmw * ghmd
fthw: gbbv + ldpj
gpph: crgp + zmzr
shns: ncrl + sllm
pgwt: rslg * mmfm
ntpv: 14
dgpj: wfgv + fqgl
nlbf: 1
qdqz: 10
wtsr: zvvt + gdqr
bvtg: 3
gzpb: hlbn * jmwj
dttd: 13
mgsr: 4
ltvp: 4
gnds: 3
rgvn: pspm * gtmc
cfnn: 2
fwqf: 3
jmht: 20
zpbs: vcdc * crhq
mqjn: 13
qbqr: tjpb + nzvn
bgsn: 6
qcfr: 2
ttwt: mcqc + tmlj
plhz: jcvp * vsjr
jfbd: 6
wqwr: 3
tthn: tpft * jfwb
snbn: jtfm + tcsd
qzpv: 5
fnbl: wnzr - zpnb
jlng: mvhs * wrrv
bhqj: mhvv / thcs
vcvc: hjjr * fnzs
ldbn: 2
cqds: zvdr + qsls
twzb: 2
hrzp: 2
lbpc: 4
fcvm: 2
qdcg: 3
lslf: 11
ccnm: 11
hgsj: 3
jnnw: jjlp * wjfs
grsn: gnwq + pfzs
jbln: 1
tlgj: jlfd * pgpw
qdbl: jswq + mrlm
lbmv: 2
mdvf: vtnq * shwl
qhgr: cqww + wgvd
hqps: pzbp + jplt
bvvj: 2
rtww: 1
msrw: 7
mgcb: nwtq + vfcl
wqdf: bwrv + mdpp
tcnr: 19
mdtr: qpch + zgjg
dlnp: pdpq + cfgc
vfzc: rcff * mbbt
zdgb: whld + tldt
mvfg: 2
svhs: wstv * gzpb
nsgb: nvrv * jvlw
vjhl: 2
sthp: whfc + pwdz
jprj: jttc + frzh
vrhn: znnl / jcms
ndgp: jwrw * jzdb
djlc: 3
pmzw: mttv * gdzq
cmnr: jcrl * ggzd
lrnd: ltlc + qqvf
lvff: 2
pfgz: 18
nnpl: 6
mpfm: 3
qpbj: 6
vtcn: 1
jtpc: 5
vqmn: 3
vwqb: bghm * pnfs
vpmc: bzcs - pszd
llpj: czgz * pcbt
wsfp: 2
dczv: 3
nfwp: qrcq - wnqh
lfwn: vfzc + vwbc
zghq: 2
nqpm: wczs - lrfs
gqbz: 2
gfwv: 3
gngb: 2
jqpl: 10
wmrv: cfqh * ltbf
ltgl: 3
dndn: 1
hrvn: fvjf * qrdj
jtnm: gqwj * ppbl
zngz: hzjg * lhqm
rmmm: 5
bgcq: 19
lhsm: zhwp * wtzb
dvch: 3
scqd: 2
ngjw: 2
jwfw: 3
htcb: 5
znpf: 3
bghm: 2
qthb: 16
zjtw: bzht + ttwt
szwr: mcnq + nlbf
trjn: nmhm * tqlw
crgp: 1
mvrr: zwqp + nqwn
qncs: 2
msfw: qtgm + gfbv
gzjr: dvnt * czbh
rrtl: 5
rvlb: lvvj + jnbm
fzfl: qjqv + dzlj
qpbt: 2
cwgg: zzzs * hbfm
qbpb: whmg * twrh
pjpv: 17
qrcq: nqjt - qthb
csmq: 4
hhvq: mvwt + jtvp
pwjd: 2
jjlp: bphq + svvc
nsdq: cjjl + ndqp
fzlg: mzgg + qrrd
zrsr: vncp * tdnf
jttz: trpd - plmq
lgff: 5
czrp: 4
qdfr: 2
smbz: dblw * dlpb
lrmt: 5
zwwq: gcmn + gsfb
bmcl: drjg + lfhj
gnwq: 18
jdzj: qqcl - zswm
vmvn: grgz * pgjv
qvrv: prrv * npcp
ggzd: 3
mrsc: grwd + mbbg
gcmn: 12
slrh: jmms + gspr
mvnh: mdsl * lcsp
mdsl: 11
mvcr: 17
shtt: 1
nwtq: dscb * jssr
fqtg: czrp * ffvj
hlws: gwnw + gmzc
fbnq: 5
cnqg: 3
cqcq: 2
rsrn: hhwt + jlmf
fnlc: hcjb * sztj
mfzd: qrfv * nqzf
bcqw: 19
hslm: 3
zwpw: 2
rhnn: 3
gbgj: 4
gfbv: jzbw + zzdm
dnpr: rbmg * jcgg
nsbb: 16
pmvb: 17
tbbn: ccnm + llgc
fnqp: mnvz * qqfm
qmdq: gsjl + qztm
rmmt: 3
qrrb: 2
tjsm: 4
qgpd: 20
mnzg: wwgn + vnrq
lrcp: nlsw * schg
bqnf: sqfw - fcct
dfvq: bpct * bmdw
cbmt: lnjg - pmsh
vmjs: wfjz / mgfs
znbf: dzcq + nftf
ltzw: 5
nzbq: bsdb * hgwm
jttp: gdhq * qscl
bsvz: 3
svrw: 2
fmcn: dsdj + fdhj
smdt: 2
nwsm: 9
fsfb: 2
rprr: 8
flvq: 14
dmpn: mpbq * tmlc
msfm: hrtr * bhpj
vggc: brgz + gsjn
flql: mwqv * rprp
qztm: czzf * nqgb
lmqh: qdtb - scsn
rpjz: 11
rdgs: 3
drhj: 2
czlw: ppjm / lsrl
fhtg: bbpf * wtbp
wtnz: 2
cslt: 3
zfft: pcvs * zwff
scsn: 10
rdws: fbvc + thgf
sqpr: 8
gvsh: 2
tlfc: llrd + bzzg
qfhm: ccbg + wmpt
drzh: 2
cbfz: 2
lbgc: nsdq * cpgm
zpnb: 2
mjwg: bqcp + vqsf
jdmf: wjtn + lsdf
mlqg: 5
jqvb: clmm - zrlc
rvbh: 2
mmqp: 11
rvjp: bcbg * sdgd
fvhw: 12
brgz: 3
vlpj: 3
tqgm: svvr / nmnr
gfqd: 5
fvzp: 11
dqmw: 4
zwlf: 8
jbqh: 10
qsnb: zjtw - gwtj
lccm: 4
cfcz: 4
cfqq: vhgb + thrl
wqqr: 6
sgpp: 4
bzzg: grjc * znbh
hqpn: 13
pjth: 3
lfms: tfwb + wqdj
crvv: psrw / mvwv
nnjj: 3
hnwj: qqdf * znfh
jzbd: glpz + gwcc
vcmw: gsrs * fzlg
gzfz: qbpb + znwl
shlf: 15
qctt: 11
fpvt: hhtl + ttjz
mtvd: ddgb * tvst
dmtj: vwqc + dgcl
fmzs: rbmz + qrtl
zwff: zwjm / ngjw
crtq: mtvd + qphd
bbvq: 13
jbmb: 19
jmdd: rgpm * wqnd
bfhl: spvw * hgcb
qsls: njvb + sqtn
cggg: mlvs * fdpn
bdsh: 10
btlz: 9
zwqp: tdrf + cpfw
bcqh: vgrh + mmls
hjph: qprt + bdzw
gdwd: scmn - crvh
ttpw: hmrg + lmwp
cbtb: 2
swwc: 3
jwlw: 6
mszl: 16
zjjl: zqcz * wmlt
swvg: hjqd * gwhl
rddc: jsgs * hntv
gfnl: 2
dmzf: 5
rhqv: 1
rnbj: 3
gpgw: 2
jgwh: 8
jfth: 13
wgcf: 1
zzcv: 2
jwvt: 2
fltp: rdfz * clrs
cqft: 4
fnzz: 1
rdlz: pwvs * trmr
crrs: 3
jtvp: qvhb - dcqq
mbvw: 2
dnzp: 2
jsfw: 2
vstm: nfch * vdml
bwrv: 1
sqfw: mpgd * qqfq
fhqg: 2
hgcb: qncs * cgcv
gsjn: hccq * pcnj
bbpf: 2
sfzm: 6
wtcf: 10
jmwj: 4
vvsm: mpjz * zhjt
qldf: 2
wgst: bvbp * lpdv
hgst: 3
cthb: 3
mzql: phhh * fndm
cqcd: hzpf * ghjs
ldqw: 3
ftpd: dbss * ttpw
wjwr: 5
bhvq: zhsh + ttll
htpj: 2
fdtr: 7
qqcl: 19
cqnz: 6
scgt: jhhz * jmhz
smzz: 3
hfzd: qwgv + qfzf
jtsr: 2
fzsv: wddr * wqgj
grjd: jrjc / pblm
qndt: vsbb - fptz
nvsg: 4
mmsh: 6
nqlm: btrz * nhrv
cfgc: jgts * fmpf
fbjj: qrqf * jwlw
zndd: 18
wvcm: 19
zwsd: 6
vdnj: 19
gwvl: vvvs * cpph
frbf: 2
gwrz: 2
jdbg: jprj * wldb
bdgs: 5
nmrd: 3
wmql: 8
gnrc: 10
fjsm: rsnf * sgzq
tcgz: 13
cgjz: pmgv - zcdn
fdcm: 9
lzds: blfp / vjfr
vsnp: 2
bhnl: hdvd + slrg
mwqv: hmnz + dvbz
vbrr: 14
gpfn: jtbv + wcbd
cshj: 3
lsdf: 5
mvbm: 1
zwbt: 2
twzh: 17
jpzt: fwgt * vmbq
pfcc: lgff * rrqf
vhbs: 16
vqsr: gbfq * vqph
zndn: 3
zhwp: 3
mnvz: 9
cqzp: 9
srwv: bqvz * psjj
vwmp: 5
splg: tvvf + sjvz
gdvf: 5
nfqn: 4
njgl: fqdq + fwqf
cdlj: btqz * gqbb
bwgq: 2
cmsw: zvwg + tbfv
fjpl: 2
vzcw: 5
dhwf: 2
srmw: 17
jdqz: vlnn + vdgc
dblw: 2
cpdc: ljmh * lmfr
vqzw: mdbb - zjwj
jwwl: 4
gvln: zwpf * jpmr
svgg: 3
pfzs: 1
zpwp: pqcl - lfrh
wtbp: zvmr / hrrq
vrjz: rhfh - rwnq
jljv: 2
sjvc: ppvr * fsmg
pmqh: 19
jlmf: 4
wwrj: qzvz * jwgm
jzqz: 6
jdhq: 2
vmhj: 3
zzzs: jttz + lbqs
lplq: wstr + pqgd
pqss: mssg * clzq
jgwl: qgvd * cshj
jgql: rgnm * bsbl
rhbl: 3
qmfg: 8
vnrq: lphj - pqcz
vhtr: 4
hsld: ttzz * fsfb
tvcz: 4
qqwm: hjph + ndqd
dzlj: rdgz - tzpt
rbbp: wwhg + wtwl
vtnr: 16
gngz: 5
dnlb: gvlc + rrjh
ldpj: 15
bqvb: qgfg + nnrq
twnv: 3
pmcv: 2
trfl: 5
vqzr: blpb + mhfl
ncpp: 9
lvsv: 3
tfsh: zndn * qpnj
wtgq: 5
vgdd: lplq + jcqh
mlmt: tvcb * bpfh
tfvs: 14
spfw: 5
mpfq: 2
jznt: 2
znnl: pdtc * bnff
qjcg: 3
wfmq: 3
jtfs: bwvw * tjsc
hcjh: 13
mlbv: fzfl * gvzf
qcfd: hvhg + ghqg
cmrl: bdgs + cdsv
fwgn: vqsh * mnld
fpsd: 1
gpvp: 3
vlzp: lctl * nwnm
qrrw: mblj * cdrh
rcjz: 3
vjsf: wmsq * dfvq
jgch: srhl * bcvp
hcsj: pfvz * mhgh
dnlr: 2
dgvd: 6
bnjn: bvbr - jzps
ljhm: dtch * gfgw
dvqd: gfgf + mvds
vgwz: 20
brjb: 11
ncwg: hjrj - hddh
hzqg: 4
jsgs: gpfn + dqzf
ppvr: cnth + vccz
hjcf: wfhd + dlnp
lsvl: 1
dhvv: 14
hpwv: hjgg * spnm
djqm: 6
thrd: rchd + wtvf
bvbr: sndq * lqdb
pcdm: 5
zhfb: 15
lcbf: 2
cshz: 2
wpsr: qdbn + mzwj
cjvf: zznb - mgcb
hfhr: jpzt + bgdw
dnjj: wnqj + tdcd
fzrw: 1
hprp: 1
nhvr: 20
dztc: 2
vlqj: tgdd - ldtc
crhq: dwbv * hmpg
mhhf: lmvn + rcwl
ttbs: 6
phsw: fmtz + plch
btjs: 3
qvfr: 2
nhnr: glzf * grtm
dzcq: 6
jsvp: mqjn * ntjl
fzwz: 11
zmlb: 5
pzbp: gqcn + grhg
vlfg: 3
htmw: slzc + hgfn
dfmz: mmrq + ghff
wnqj: cmtc - ljjz
mwnr: 3
wjrw: 2
tgbd: pncr + cgdv
qbgg: 5
tgbm: 1
tgcw: 5
flzg: 5
tfjh: hqwv / nnzg
zcdt: 3
bphh: 2
hqsn: tmhp / bqml
ccmg: wpmq * fcvm
mqjw: qvsh * tmbn
jhlt: snpb * lnth
vwfc: 8
wjgp: pdlb * mpfm
zwsq: 13
rcll: 5
hpmt: pchp + gfjj
pmqt: gcmz + gpdn
jzbz: 15
csnb: 2
qbmm: 2
wtms: 10
fwbz: 18
qsrp: wlrt + llts
hfqd: 9
slrg: gsdl + swmj
dqzf: glgl * cpfv
jcvs: tncb + qvtr
nqqj: 9
pvdt: 2
qrwj: 2
mpgj: 11
vwbc: dczv * jfln
zqcz: 3
cdrm: rnbj + szbc
pfvz: 3
wzjh: 2
czvw: 3
fwsd: 4
dbpb: 4
fmpf: 2
hstn: 3
jfpn: 19
smmp: 3
swrt: 4
wwgn: wmwm * dmtj
rzrq: 8
vptb: 15
tdhm: fsbh * znpd
jlfd: 4
jpmr: ctmj + hlfl
nlvz: 11
nccg: mfrl + twss
bmpb: 4
mzwl: 2
sqlw: 2
rcff: 2
tbrn: qbmm * srlq
pwft: tdjs + gmfh
lhzf: 3
gbdb: 2
dzmj: 15
gfcz: 2
dfct: 4
djcq: qmdq * fmgn
tzlg: tqhv + zfmj
jzlv: 3
fvlv: 6
fwwc: mbhm + zwmw
mvwv: 8
hjgs: 2
pgqb: 2
sntd: 2
gbbh: nzdn * tbls
tzmz: fzfj * mvjp
wvjn: mwjs + jscv
hmnz: tcnr + qhdg
fnvs: 4
qgww: 2
jjzg: cpzd - dsft
rfcb: ndgp + tvtv
bjhg: vmdn * dbct
vjvs: tlqp * nnbg
hfdc: zvmm + rjbn
qcgv: 5
qhdg: mbdh * rqbd
ndnm: swdl + hzmm
gswz: 8
rbbl: 15
fqhn: sdwn - wmzc
wgcl: btfv * znpf
dwrm: 5
srcq: zzwp + bvrh
cbps: 3
jjlf: jfhl * fvth
hvpn: 9
cgsj: wcdc - mhhf
mvhm: tqbz / scbc
zbns: fqtg + thdn
dfcd: lslw * frhc
jmwt: dzcs * qlsl
cqvw: 19
tpcp: 2
rbmz: vphj + pnzl
pspm: 13
wwtg: tsqm * czvw
ftsb: bhrg * jwvt
nwwj: rlsn * htpj
clcn: rvlb + lczr
jpqg: mqwh + tdhn
cfcq: sfjf - gmbb
qfcc: 19
jqfb: phph + bdsh
thmw: 5
jgwz: zvvm + vbtt
cvps: 3
bdvb: 3
nncb: shns + vvns
jmwp: 4
jscv: 2
bqvz: 9
mdpp: 6
gcsj: fsjw - gqpm
rlzn: 2
ftsg: tqgm + wwrj
gtrf: ttfm * zmdn
wjfs: 2
pwdz: 18
cdjj: 2
rtvv: 10
vzlv: gvrv + rdqj
whld: 5
crbn: sfmv * zscf
vwcw: gsvd * gzfz
jcjl: 3
qlnf: wtwm + cthb
hslh: 6
hddh: qjjw * zsqf
bbgj: 3
hbfm: jpbq + fpsd
ltgt: 2
ngrb: bhjz * bzvl
mnrp: whbn + rjls
hrbz: dbvd * qzrq
thld: 2
llhg: 6
lczr: phjj * wgjl
jlgs: 9
btrz: bpzq * nncm
fddh: mmgm / cvps
pvwj: 7
cmdv: 4
mhvv: 14
rtrr: mtfv + vldr
pdtm: 1
vbpr: 2
bcmh: crbn + pcls
ghqg: gdqj + fzrw
ppgq: lhgt + wpsr
pltq: 20
mrlz: rvsd * cqcq
pdtc: 2
jpnq: 3
qlsl: hptr * mjwf
ngzn: ppld * hbrn
gsjj: 5
rnjl: mbvw * bnwp
wmlh: 18
sqds: twzh * mvsl
vtzh: rqqb + bztj
qgsd: hphz + zhfb
dtwm: wffd * llmw
czfp: 5
pprd: 3
rgnm: 3
zvwq: 10
zdqb: gbcd + mrlz
rrzf: slrh * hsdf
hgvc: dcwz / svrw
gssj: vhbn * fnmq
lpdv: 6
rbqn: smdt + vrcm
clrs: 4
dgcw: rdwq + vpjp
bnff: 15
czww: bbvt - grjd
hrrq: 2
rhns: 9
fvnf: 3
jprh: 5
sjdr: rlql * lslf
ttfm: tcvp + dnzp
jplt: hqzz * bmpb
vhbh: 2
msmh: 13
vzfr: mvht + dprs
rldg: 3
jcwg: hflg + zpnz
nthl: 3
cfgm: tllh * pdsn
vgpq: fddh / mdrv
scjc: zhlj + dtjj
tldt: flvq * jjdp
jqvp: lvff * jmpl
jvql: 7
sjpj: 3
sqfc: 1
gsjl: zjpc - dmpn
nrjc: svdv + hrbz
fgzl: 1
wtwm: gthr * ccps
tcfl: 7
czpd: 2
pjsq: zdqb * wzpd
mmtg: gnjf - pqbt
ljpp: 3
gbws: cmcq + lqsz
zndb: jzpw * hpmt
qwmj: 14
jftd: 2
cnlz: 2
zdwh: 15
svvc: wtnz * qdcz
qjsh: vqgc * jthr
zvvm: 5
cplp: 4
bmrd: 16
vqsf: zndd * zcmt
hmpg: 3
mtpg: swhq + hflq
glwd: 2
tmsg: 1
hlfw: 2
vnlv: wzlt + ljhm
mtmh: rjhw + rnzv
sprc: 15
zlfg: tfgl / pwjd
jtrs: qfvb * mzwl
btzc: 2
fbhn: 3
schg: 6
bhqg: vstj * dqvt
blbg: 2
gpsn: cthw / vlqj
fbgb: njgl / mjvv
grjc: 3
wzzm: czlw + vbqj
zcmt: rzrq * bldj
dzln: 20
jhgh: tgcw * smlf
rcnh: qchz + rdrz
cpzd: bcrz * rzqm
cgcp: 2
rdfd: 3
hwhv: srmw + qlsn
qmsm: 6
zzgf: fjsm + cccs
zfrg: 13
jhgr: fdcm + mqpd
scbc: mbht + szlc
sqqb: pmzw + cdrm
hhzv: ftsb * vzrz
trvl: vhls + zvwb
pqvn: 2
jssr: 11
nhff: 2
mpql: 10
rjjd: 6
nqjt: lswp * ndrm
rwpr: rwgd * jwhc
zwhs: hcpt * bbqz
wrrv: rwsp / bczv
smdc: mfzf * lfss
tplg: 18
rbtz: jhmj - rtfr
zwjs: flql + ftql
dttj: lsvl + clrj
pvln: 11
lsrl: 4
nmnr: 3
bqcn: 2
bpzm: zjdg * npql
jnmz: czmz * lcbf
tvbb: fvzp * rjlr
nmdb: gmvf + hfhr
gqbb: jzmj + hdvl
hzpf: 3
dtch: 9
qtfj: gfwp * vfvh
cjbb: gbpq - qsnb
qjbq: blqv * vlfg
ptwt: 2
mpnb: 4
tnfh: ggfc * mglm
vjzg: 8
tfzq: 2
bpzq: 3
ffhd: qlhr + dwhz
bbhf: 7
zwrd: 17
clzq: 3
jsmw: 11
nntv: hnrg * sjlc
zvlw: pbzh * fsrz
djgt: 4
gpwg: mdwn * vbmq
pdpq: 2
szbc: 10
bmwl: mclc + dttd
phdw: qjcg + lqcl
fdpn: 5
jcvp: 5
tmrl: 19
rbjh: 18
lmvs: 3
hzcb: 5
zqbv: pcvh * qdrz
bbmg: ljrd * nqpm
wznh: 5
nqdn: 17
fmgn: 2
gsfb: 1
hrvl: zfrg + mgdj
srrg: nvvr + ntqd
fmjc: 5
sprb: vnvj + zhzn
tsqm: rbbp * rmcw
jzpw: 2
lzfd: 5
wbpr: bszt * rsvt
ltlc: 10
mrzg: lbzr + zmrq
dpms: djcq + qqwm
jmrl: tzpm + bcpc
wnzr: twdp * pjth
nzvn: sjwj + clqc
rqbd: 11
cnth: 11
tzvp: 2
wmwm: hrzt - bgpp
chwr: 4
gjrc: fmrt / qwcm
cvpr: djnz * wvcm
jcqh: shtc + zpwp
hqfm: whgl * sfbt
ctjf: 5
jdpv: mfsl * hfcj
vtnq: tgrv / jhlw
nwsv: zngz + mlbv
ggfc: 3
jbfw: qtgf * qpsj
lnrw: 1
ddgb: 14
mrgh: ljpp * ntbd
spvn: tcpl * pwws
qwgv: 9
nsjd: 7
wcbd: mvfg * znql
qpch: mjws / gsjj
mwjs: 6
hnfc: cbmd * bcdj
gtfr: 2
lshp: fwcz * mfvv
bgwh: 4
fsjt: 2
wvns: 6
lmtw: 3
lslw: 5
nffr: 2
fmmf: 3
lfwl: plhz / rwld
bhpj: 2
czbh: 3
hblp: wrlg * fsdn
tdpj: rsmb * rdgs
zgsr: cgsj * sjbv
njfq: 2
sbtd: 2
qwgh: ppgq + czzv
cdtt: 2
tghn: rrtl * cfjj
wwvq: 2
mjsd: 3
pmgd: 2
zwpf: 4
cqjc: znbf + twdb
wznc: 3
bsdb: 2
jnrr: 15
pmmm: gfzl * thld
ffls: 3
jcms: 2
cpph: 13
gtrg: 11
tlbj: hlzs * fsgm
mmgm: tfgn * ftsg
njhn: 5
wllc: gnpq / hrzp
bzcs: vbcs / rnjl
sfjf: mlmt + bsnr
ptsl: nzwr / cvrw
cbqc: vjfs + llpr
dbss: 3
gbsw: 5
vmdn: vmwr * hvvh
rhqq: 2
fvtb: ltgt * trrh
gbpq: dndt * bpsv
bhml: 2
znqm: 19
gjwg: mjsd * fnvs
lqnt: 6
zvmm: tvzz + rmds
ndhg: zjft + lngw
zvwb: qrrw - hmvf
wrrr: 4
mhnw: pnbv + nwsv
mbht: 5
swhq: bcgm * mfmt
tdft: 7
jbjs: 5
vnth: 5
jjvh: 2
ppcb: 1
chdq: ztjh * mbrv
vllw: 5
qtsz: 5
rdnm: 2
rlsq: qcfd / bhml
cvhs: wftq + djqm
vcch: dhrs * vddt
wftq: 1
mvzt: ftds - bszh
splr: 18
qdqq: pchn + sbhl
whmg: 4
mflq: 2
hcrw: hsld * cbps
ljzm: 3
znpd: 2
grgz: 3
brvp: hlws / nprz
mtwf: 4
mwrq: 2
mlpn: 17
jpsv: vdnj + vdzm
bgdw: hjdn + hqps
pqhj: 2
rqjv: fnzz + cpdc
wtvf: 5
gtpg: 2
fnzs: cntn * zzjq
tjsc: 3
vqsh: 3
jwbm: 3
mvll: vmdl + vgwz
fvsf: 4
vhls: slnp + cjvf
bzht: vvqh * whhw
fshp: 13
qmbr: 4
wbvq: 3
tzpt: tbzc + cwgg
rshh: zfhq * rjhf
dscb: 3
thgf: nlds + hdgr
dpdz: vdbw * vwqb
qlgg: hssf * qhgr
jdgl: vvbc / rsrn
vdgc: 5
pddl: bgrv - gnrc
hjdn: gbdb * bgqf
prvs: 2
lqsz: jprl * qrhq
qwjc: qzqb - tbsp
nfdq: psjg + jtfs
cntz: 4
rnbg: 3
tqlw: 9
ldtc: 2
hgfn: tbrn / gtfr
zcsp: lszb * vdhl
nnwd: dnpr + hclm
qrfv: 4
hpld: 2
tmhp: szjz * cdmj
jhpl: ldbn * bgwh
chsm: cmhw / lcsh
nvzd: lpnm + zdzg
jjdp: 3
mjzj: gpvp + vhbs
qchz: tthn + nntv
srrz: 3
zcwt: mvtt + wjpb
bvrh: 8
ctmq: mwth + dztc
smjt: 9
rprp: 9
pfrd: jwqn + wznc
ssgd: 6
jwqn: 4
sbmq: 18
ngdh: qrwj * gjvr
rzzg: 13
bpwb: 2
prlt: 4
gscv: lrwv * gzpl
ppwb: rpnw - fgnl
bhmc: frhm + nqdn
bbjb: mrgh + qlnf
wfhd: vlsv - zmls
vnvj: 20
jjln: 17
psrw: lqqg + bdtw
zvvt: jdqz * rbqn
hjzc: 2
qswt: zlwc * rcjz
hptr: jlzl * vfgm
qjjz: gnln + bcvs
bhjz: 7
snct: 4
wctw: tmzn * srrg
ljzj: 20
hdnr: zqzn + tvsg
mvww: 6
vrqw: 3
wllr: 18
zqzn: 18
vchp: 14
bdvs: nhpd * nvwn
rtqf: 2
cgzc: 8
zqdz: 2
tldv: 6
zfzf: 2
vbgh: 6
gnmg: 5
nhbs: pgqb * phcb
bztj: 2
fshr: gnmg * fjpl
tvst: 2
tmbn: 7
spvs: 5
wbjp: psln + jjln
ghmd: 5
fwcz: 11
nrpm: ztwz * rnnv
jsfd: qplw * ztvl
tlhc: 2
nrbd: 3
swdl: zwlf + lpdq
rdls: 4
lvjg: qwwn * gdjd
tfct: sgqg * hcdh
bqvn: 3
spvf: 3
mzwj: sbwj + qchc
wrtc: 13
qwpg: ldhp * pvgg
npph: fbhn * zmlb
dbzw: 2
mqmp: 13
nctc: 2
snjg: 4
wbsh: hrjt + wfrr
rntw: 9
wbqs: dwcg - slzp
hczw: sddh * fzbq
npcp: qfzs + jmwp
rmhf: 2
gspr: mgng * zjjl
zmrq: 4
mbvh: 2
zslc: 2
tlqp: 2
qwjg: pcrw + phtw
mcml: mtmh * rrzf
tnjr: wcst / qmsm
rsnf: 3
npnf: gcvc + htcb
clqc: wvns + fthw
wzvw: 5
gvch: 4
vsjr: cjrt + hthc
zwld: bzpt * rlwp
ghff: nmnt * zwbt
tdlt: 2
ngpb: mtpg + gcsj
dslb: nwpg / jdgn
mztb: 4
fcml: 2
tdjs: 4
gqpm: qbhd + jdpt
sqms: vtzh / cqmv
qrrd: fdtr * mgrm
chgr: 14
prmt: pgwt * zcdt
fvsz: 2
vsjd: 4
ljqr: 4
gqbj: fhtg + ghjf
mvsl: 3
nnhq: gssj + mjmr
lntq: 5
pplw: bsqd / svtt
zjwj: 3
whsv: 5
mssg: 7
mqsq: 2
zftz: zplv + jpsv
stdg: 2
vqph: 13
znfv: 1
cbnw: bzdl + wzdg
vdpd: 2
qvsh: 13
ljtz: twfp * fvnf
hcdh: 11
tgrv: mqqc * nmbd
mcnw: 2
zhzn: 9
mvhs: cslw * zzzj
qvrn: vdzn + snct
wmlt: zqms + bphs
zwfr: 4
njcr: 3
phhm: 6
hjjr: 2
ncln: 10
qrll: cpzs * sbrg
nwpg: vfbz * wzjr
rrbz: shfg * qzlt
vgpg: 11
mfhl: lfrw / czpd
rdwq: gqbz * mnrp
whbn: dfcj * wvdd
pbrj: 2
nqgb: vzbr + gzjr
gwrf: cssd * vqzw
mgng: 3
wldb: zrjs - sdwf
wtld: wqqr + cfqq
ffzh: wbqs + hssr
nzjh: jpjc * mvwd
ttjz: ccrw * cqnq
mctp: lvcc * sbnz
cpzs: 4
zjpc: tgch * mctp
rbgr: 8
bqdw: 3
lngw: pnqq + vzcw
vphj: 3
ffps: 7
vfcl: 10
snpb: 2
zrnn: zqdz + zlrp
jtdc: bnsn + pwjt
wfjz: wgcf + wbpr
nnzg: 2
bcvc: 5
gztv: smwb + gbfh
glfh: cwzp + fhfj
lmbg: scjc / fvhr
gflv: 3
prhj: 3
zjft: mphg * vmvn
thrl: 12
btrt: 5
lddr: 2
jhzp: 3
jccp: vpmc * ljtz
vfnr: 3
mhgh: pzfq * ltgl
smrm: pptr + hght
fhqw: 6
rzqm: 7
hnjl: 3
sztj: 5
dtqg: pvpw + ngpb
gmfn: jvfs * fhdv
pcfr: 2
tgdd: 9
fnrb: 1
vcpr: 4
jtpm: 4
szvl: lnln + zgsr
hggq: rbdb + clcn
nvzw: 4
gjvr: hcdv + vtgz
psmr: ldpt * dbmw
vtgp: 3
mbmj: mmtg * mjwg
gnjf: bbzv + sczv
bmlw: 4
prrv: 4
bgqf: jpql / rzmf
dhwh: 2
zvwg: hbmv * rpcg
rsvt: 17
wchq: 2
bcrz: 6
mcgt: wpbn + jpvl
pzfq: rdrc + jzlv
qgws: swrt * jtsr
lvcc: 5
rfdb: 2
wmms: dnlb * qbgg
qcrb: 3
nzhn: fvsf + gzvr
jcmz: gdwd + zgfl
wslm: 3
zfhq: 3
zvdr: 6
fzgz: 9
sjwj: nwwl * jlmd
thdn: sqpr - fnrb
zjpl: dqbt * ccjl
mccv: qcfm * gbsw
scmn: mngb * bsph
mwnl: 2
lsdd: hrvl + sttb
rdfz: 6
tjnp: 3
dwbv: zvvd + hwhd
vrbd: 9
tdvr: 4
cnzd: qzll * czfp
srvc: 1
nrln: vgcv * fmzs
mmdz: hsrp * rldg
dnhh: tzvp * fwwc
wfrr: mqrf * gfwv
fthv: 2
hgsf: 5
fvth: jssl / hvmd
gcvc: 2
nqwn: 2
tfqn: vjts + hplg
rqgz: qwqg + fnnp
wqbj: jvpg * rfnf
qgmm: nhmw + bdtf
zdld: 2
flbh: jvqw * qhgv
zfvn: rdlz - ptmj
czgz: 7
nzdn: 5
mphg: 2
mrvd: csnb * tdft
wqnd: cqrf + btfn
dwmd: 17
vsjm: hpwv + nrpm
mncg: rwcs + whlc
mhgl: 16
fndm: 3
gmmd: tdsb + bjtj
gwfv: 4
sddh: qjsh + mvnh
swsp: cfrq + lshp
fsbh: mbhc + dqwc
qdrz: 9
dcqq: 1
clfz: jswt + rcfh
rhdt: vsnp * brjb
lrfs: ncrn + mzql
nnhn: wcgg + drdh
njvb: rqmn * jsfw
bsnr: jbpv * bdvb
bqcj: sgpt * hslm
qfzf: 9
gzht: vllc * zgtn
qrhq: 3
bfzz: rpjz * cgcg
qprt: wbvq * jhpl
zldg: lvjg + cvpr
bhcq: qgpg / zsnm
cqts: 2
bbvt: bnjh * jnrr
pnmj: 4
cthw: mmdz - jqvp
vhgb: tmsh * ghdj
cqzs: 1
glbw: 5
lpwv: 11
ngqh: ttww / vfnr
bntv: 6
pszd: hbpn * wmms
htrt: hgsj + gdtf
fmgc: nhrc - jnnw
wpvl: 2
fwhf: 2
vrjq: bcmz * fzgz
hzmm: hsvv * rmmm
lfvz: mfhl + dzln
gbbd: jzzn * jgvm
zswm: 1
pqcz: fcqb + srrz
jhhz: 9
sfpt: hgjl * rlfr
mmls: qjfn * mrzg
bnbw: 4
nmhm: 6
jdpt: jtrs * fswm
fvhr: 2
tbnq: 4
nhrc: pprd * sqds
sbrg: 2
nsgj: bmwl * wslm
npql: jzpz * htlp
llts: fgzl + lqnt
lclc: 2
pzzz: cmrl * sltv
jgfp: 9
dzcs: dpms * swwt
sjrb: 2
sjfl: bvvj * flbh
tnjw: 2
gpzm: 4
dndt: 2
vbtt: 2
jrlq: 7
vzlr: frbf * lhzl
mfnm: 3
thmv: 2
sjbh: 2
lzsn: 2
lmlq: dbtg * ggjm
jhrh: 3
cqww: ccfb + bfwg
dgpv: nvzw * jznt
hbpn: 2
vpqg: 2
hfzv: 2
pffm: 2
dqvt: 5
vvvs: 15
cwpr: frwm + qdqq
pptr: rmft * vnfm
jfqv: 10
pmsg: 5
jtfm: dvch * msfm
vdcr: wrmc * tbbq
pzvb: 18
rbhj: 3
vvqh: 2
qcns: pmqh * twmg
hbmv: 7
bnzn: 12
zlrp: 5
rmcw: 3
zwjm: rfcb * zfzf
fzvf: 4
fwcr: srcq * pqvn
sfmv: pqhj + lzqv
bsjz: wrrr * gpzm
fhjr: bnvw + tfzd
hvhg: dttv * ffcp
fvjf: 5
nfzl: dpdq + cqcd
jpjc: 5
vwcj: glqj + mdvf
zbwd: pmqt * jgfp
gsdl: 4
dbcz: pmlb + jlsb
jfwm: 5
tvsg: 20
drdh: 17
jzcz: dfhc * nmbw
rpnw: ngrr / jzqz
wdlb: jdzj * vrnf
ntfz: 10
dsns: mfnm * cdjv
zszq: zppd / lmtw
chtg: mcqz + cbtb
zwbm: stpl * vhsp
qfzs: 4
mvwd: slhg + mwbr
hdvd: 3
pnwq: 2
fznd: 9
fgnl: vhtz + nwwj
gdhq: 3
gmjh: 4
drfz: 2
qvhb: gppl * vcpr
tsfc: 2
wzjr: 2
hzwc: 4
vvns: tzlg * mpgj
qtgr: bmrd * pbrj
bgbq: pjhb + dfmz
qcfm: 7
shwl: 5
jlsb: 18
dvnw: pnhf + nnhq
zjpn: cqvw + pcjt
bcvs: 1
pzqp: 4
dzfc: 11
drjg: tgbd / snwv
zrlc: 3
fwfq: hcrv * rbtz
qtqg: 2
jvlw: ggrg + wvlz
fhdv: 5
gswg: crvv - nrln
gmfh: 3
brpd: 2
qplw: 2
bfjp: jdhq * trzq
vtbg: bbvr * qjzt
sfvv: pqss + pzzz
gmbb: vllw * bgbq
bpwd: 4
znvl: 2
sndq: 2
cmtt: 2
sjlc: wzjh * sfvv
llgc: zwwq * nhff
llpr: 3
mmfm: 3
hqwv: mbvh * gztv
pfcr: 18
zwbh: 10
bpgv: cgfg * hnrr
zvsn: 5
prmz: ljqr * ptrg
scnd: 2
zjhg: 2
zgfl: zwjs + jcvs
bphs: 1
dmrn: hdlh * tfsh
czdp: gpwg * vzmt
mftl: tfqn * pffm
qhnn: 2
ljjz: ppjv * vbvm
btzj: 5
fwgt: 2
fqwr: dqmw * wvbq
nlsz: vtgp * dhjb
thgn: 3
ldhp: gjnw + zqbv
rzfm: cbqc * pmmm
pchn: jgwz + jqgl
zpnz: hdzp + sggd
fwcl: 5
sjlq: 2
qpnj: bfst + tjsm
zznb: prmz * ltzw
lqqd: 5
njss: 6
ntjd: gtmv * hppv
lhqm: mcml + grpr
dqwc: jpth - ntfz
rdgz: mpbh * nvzt
wcst: jccp - nnsh
bblf: dnjj * qgmm
nwwl: lqqd * zcjn
ttww: vlcf + pplw
fjmh: 12
slzp: mvrr * ccmg
ftds: vwfc * pcfw
hjgg: wccs + gzht
bghz: 5
gdzq: 6
phcb: 5
phtw: wjwr * hlfw
wvbq: 2
gwcc: 16
jssl: bslv * hgrg
mtgv: 7
hsrq: 5
wtzb: 5
hcdv: dfnh + srvc
fgvr: 2
nfbj: dqzg + phdw
pgnm: 3
fcqb: fdbn + tgvg
zttf: lccm * vbfq
jbtb: 9
jbmc: fbjj / hwrp
rwnq: flzg * hrvm
ccbg: lgcr * jjfd
cwdr: 2
tdhn: 1
sltv: 2
vzrz: 2
qtnb: 2
cdqm: 2
qqhc: bqcj / gdvf
rmds: gtrg * pnmj
hdlh: nvsg * fzws
hbrn: 14
nctn: 4
fswm: 2
vhbn: 3
hppv: 5
gvhs: sznl * lsdd
zqvq: 2
mblj: 5
tgvg: dzmj + fhrl
dwtf: 7
rfzr: 8
fjgw: 3
tfdz: 3
rvsr: 2
gbbv: 8
fhfq: qrdl + hbmg
twdb: 6
pnzl: 4
wjpb: ltrg * rmhf
hsvv: 2
sznl: 5
phhh: fwvn + gbgj
lnln: fpvt * ltct
hlfl: tbvc * jqvb
pcnj: hcjh + gmlb
nvrv: gbws * rdfd
bvnf: 4
tmlc: 3
ltsq: bbhf * tvqf
nwwh: 5
mqwh: vqnc * brvh
wgvd: qtfj + cgjz
gsvh: zzfb * ddqw
whgl: 2
frsz: 7
hpvm: 3
svwd: 2
cpfw: pmcv + ncln
hhwt: 2
qsdt: 3
pvfc: blvq * jspb
rsbs: 2
gqcn: 2
hrsn: zjpv + thtm
qwfp: 7
jjdn: hfww / qpqj
tfwb: mjzj + mftl
sgqg: 2
gppg: 2
hflg: 11
znwl: wngs + fnqp
szjr: 1
cqmv: 2
wmwc: 7
wpjt: 4
nqvj: hczw / nwwh
jmdc: 5
mwgv: 1
llmw: 2
gnvm: 5
nqlt: twnv + qrll
wsmc: vtnr * drbv
clmm: cbmj * rtvt
gtlj: nmww * dwtf
mwth: 5
ztvl: jgch - qjcl
trzq: bdrs + zbmj
lbqc: 16
cfnz: djtg * cmbv
jgdt: gtpg * thmw
tqcp: hblp + lrmt
djtz: 8
ttvd: 4
ndqp: 3
cprc: ndwc * swsr
rbjv: gscv + vjvs
fpfd: dvjs + bpgs
vncp: jsqn * lclc
dqlm: 2
ccvv: 6
dwdz: sqms / jcsn
drbv: 2
zqms: 10
cnfm: zzzd + lpbf
bpqn: 8
rqmn: gchc + smlv
qgvd: 2
bsph: 13
dvbz: brnb * djwf
ztwz: 8
mvwz: fnbl * btjs
wzvl: 4
jzzb: cmml / chtg
zcqm: jzhp * swwc
wnqh: 5
hbfb: 7
qscl: 2
frwm: 17
ldpt: 5
bcvp: hjgs + sdrv
pnvp: nqqj * hlfv
jthr: 2
mnrf: fbgb + glzz
shjz: 2
mfrl: 4
tcsd: mdtz * jzbz
mjvv: 2
jwhc: 2
clrj: whsv * tlbs
pwvs: 11
bbvr: 2
pcbt: 3
rnzv: bvnb * cpzm
vrcn: 2
cpbq: thfw + lzms
qpcr: vtbg * bbgj
hhtl: cjbb * rqpp
tzzt: 16
gwnw: nvjv + cwpr
jpth: lfwl * thgn
btfn: 3
mdbb: 9
ljrd: 3
psln: shjz + hcrw
jqcg: 10
rtrf: 3
vvbc: cfnz + tzvd
bszt: 2
dspj: bdtv * rqgz
mvrd: 4
tdnf: 2
lfwg: dpdz - qswt
mwbr: cgmt + cgtq
hzvq: tlbj * vdnz
vnhg: 2
glqj: mdlr * mlnj
ztfb: vgdd + twdw
hzbd: 3
gmzc: qwpg + bcqj
phph: 2
qfrj: 13
zqdv: 5
zmls: bvtg * lbpc
pddn: 14
dprs: 4
ftql: nhnr - mnpm
sdwn: mnzg / pmgd
tgbz: pfcr + rjfn
zvff: humn - zndb
zsbv: smmp * fpfd
qjfg: fmgc + sdpm
gdqr: dqlm * dtwm
rvsd: prhj * trrp
bsqd: bcbr * zjpn
rjpj: mhbm + wmrv
smgs: tmsg + znpq
fsjw: mvdz * vptb
bpct: 2
sczz: dpfb * szvl
jrjs: rwmp - ccvv
jzhp: 8
swmj: gjsd / qrtm
jpbq: gwlv * jsmw
fsrq: 5
grzl: dmzf * mtgv
bfst: 6
hcpt: vgpg * svwd
hrrt: vnlv + jhlt
vhsp: 2
wcsj: vrjq - bdvs
jfhl: lfmf + brvp
qjnm: 11
mqrf: 7
clfs: 5
pwlg: gmhm + lctd
fqdq: sgzl + qjjz
mbhc: cbfz * tqhl
zsnm: 2
bjtj: 8
mmwg: 20
hdgh: vjhl * swrw
mzfg: 12
glzf: rpjb * gfnl
zznq: bwgq * hhvq
hnrg: 4
hnfp: pwzl / fcdc
mgrm: tvbb - fvlv
hgwm: 9
rjbn: pssw * zhvf
cqnq: nbss + zmcd
hpsg: 3
sttg: hfjb + rntw
wzlt: qbqr + sjvc
psjg: mpnb * ggcv
cgjq: lqtn * dslb
prws: mgzg + hnwj
vwsm: sbsp * fzwz
mrmp: 2
qfvb: 3
whlc: wsqv * hqsd
vcdc: 10
fbnf: 3
zqpg: lwmg * hbfb
swzh: 10
nnsh: fwmf * hzbd
fftt: dzfc + cclp
bcbr: fpgw * snjg
tvcb: 4
nncm: 2
zcrb: pflw + wbjp
rlwp: 2
nbss: prws * msbn
gzvr: 3
zjhw: 2
rmnb: rhbl + lmvs
lphf: 4
lmwj: vzlv + hdsw
tllh: 3
dnhv: svrq - mvbm
tmlj: 1
znff: 4
vdbw: 4
nmbw: 2
vhfp: cwnf * hfzv
zwsf: 3
rgqz: 1
mrwn: vcmw + qtsw
twrh: zvtr + ldrg
gfwp: 2
lswp: 7
fpgw: 2
jnvd: sqlw * vwcj
gfzl: 13
qdwb: 2
mbcj: vwsm - qjdm
bvbp: 3
sbhl: 9
rbmg: gpph * rdnm
cchd: 2
dbdm: 3
mlnj: nwhd * fngc
rhrt: 3
qhgv: 16
cdtw: ffps * jwbm
tzbf: 3
flnn: 2
pcjt: 1
dcqt: 7
qnjv: csvz * zslc
jpgj: ptwt * qjbz
wmst: fwsd * dbcz
schn: 7
tsdh: 3
mglm: 3
bhzb: hfqd + mstp
cfvt: 16
mqpd: 4
vllc: 5
qzll: tbcs * qjnm
znql: ctjf + vjzq
plgj: dbdm * mszl
nwfr: hzvl + qhmc
wsln: 5
zppd: ltvp * hmjm
cszf: wdlb / sbtd
sgzq: 3
hwhd: zqvq + nvfm
cnjh: lscd + cpcg
lctd: hjzz + jbqh
hjqd: dfct * tvcz
lfrw: wllr * zrnn
hjrj: cszf * sdtq
rnnv: 8
brvh: 2
ghjs: 3
sjvz: tnfh * vqsr
gqth: 3
jqgl: 3
gppl: 2
hmjm: lddr * jttp
rgzp: glhl * rmmt
hfsv: 3
bnsn: mvww * qpbj
jngq: 9
slnp: glbw * fldt
mpjz: dgzw * jmzs
sdrv: 5
fsdn: 3
znbh: tztt + qcgv
zhjt: 2
cvcs: 2
jlwj: 2
nhvg: 2
tzqp: 3
phjj: sjfl - hrsn
pqcl: ldnl * jmdc
jcsn: 2
hlbn: 2
hrtr: pdnq / jzsz
twjf: hzcb + nlcv
gnpq: fhjr + bhhj
shfg: sgpp + wzvl
jfln: 3
qlzm: zsbv + jmwt
zpsf: rbzt + lntq
vgrh: qrbn * nhhz
dsft: 9
mvjp: 2
blhr: rjpj + glfh
qtsw: bmhz + jpcs
dttv: chgr / rhqq
bcmz: vcvd + bntv
mgfs: 5
rslg: 3
czmz: rbjh + hfdc
hsrp: srwv + qwll
trrp: hgst + hdgh
tbzc: vgpq / jftd
rtcg: 2
ptmj: fhqg * fbnq
whhw: 3
snph: gtlj - gqwd
jlqz: jcmz + mbmj
ccfb: 12
lvvj: qwmj * wsln
zzhd: hnjl * jhrh
rgcl: 2
trrf: wsmc * mwnl
tzpm: vchp * bqdw
mvdz: 13
ggrg: 5
lzzv: qwhs * fwcl
zhqm: 1
jgvm: 7
gnhf: msrw * hpsg
glcs: 8
lcsp: 13
qlhr: 1
gcsm: 15
bcgm: 3
pwws: 3
rmtp: sczz * nsgb
vmwr: 4
gdjd: gsfq * zpbb
mrfh: fmmf + pzvb
trgl: nmdb + bvjz
rdqj: 13
tdsb: 3
slrv: wwtg + hzvq
vdzn: 7
djwf: cnqg + pltq
bhhj: sqfc + zcgd
jplp: 3
znfh: 5
rpjb: 4
fsmg: 11
wvms: 3
nnrq: 5
lhjq: 12
nmww: chtr + vwmp
hcjb: 4
qlqt: 5
bgpp: vhjt + mhzv
qphd: 9
cthz: vhfp + nzjh
nhrv: svpb + vnwz
plfp: 2
dznv: wpvl * fzvf
jzbw: bqvn * cqds
qcpl: 5
wgzs: 11
qchc: cznm + vjsf
wznr: dttj * hnfp
rcfh: 1
dbct: 4
pffb: gzsg + bpfq
mnjp: ttbs * cblg
nprz: 2
zbjc: wphz * mrwn
trpd: vcch + gwvl
fwmf: jdps + dvqd
bwcw: dsns + pvwj
jnnn: 1
cjpj: lfrs * bnbw
rtvt: 3
vstj: 5
mnld: 9
dgsc: 17
bljp: scnd * sprb
dwcg: qwjc * zrzd
ppjv: dwqv * vlpj
gzpl: 9
jvqw: 4
zcjp: 3
cmzw: bpwd * hhth
lrbl: 3
wdfv: hnft + bmgd
cbmj: 3
tdsq: 16
blfp: bsdm * bbzs
fngc: vhbh * cnfm
rchd: 1
smlv: 9
qrtm: 2
hpgz: njwf + splr
gvzf: 5
lpnm: zjgr * bljp
rlfr: bbjb + rqjv
zjdg: 4
sdhm: 5
jtvb: 5
lnff: rvbh * qlzm
jpcs: vnbp + gqbj
ddwn: blgq + zqcs
sbzb: 2
hmvf: gwfv + mvwz
jdnp: 13
lwbb: chdq + tfjh
wpbn: ftpd + jzzb
wpdh: mfzd + hstn
bfwg: hslh * zwsq
bqsj: djtz * plgj
mvnc: 9
lpdq: 1
lzms: lvsv * pqmm
jwgm: wwrc * cmsw
qqvf: dqzn + rgqz
bphq: 1
tfzd: tbnq * npnf
zhsh: tjnp * gwvc
gvrv: 11
fgnr: 2
rrwn: shhn * ddjs
rhfh: gnqd * mrvd
wzpd: 2
sbnz: 5
bczv: 2
zbqv: bmlw * zfvn
bcjq: 4
ngrr: cwvf - sfpt
bzpt: sttg * jtpm
dfts: 3
pjhh: 3
zzcf: 11
jlgr: sthp * pwft
sgsn: 7
fzws: 5
nnnl: 5
pqcr: 3
bhfj: 13
nlcv: mmqb * pjpv
lgmz: 2
zqgq: lvtt + wjnq
qfrp: dspj - bbmg
zgjg: bbcr * bdpj
gwtj: 1
blpb: snbn + bhmc
qwpc: vmjs + bnzn
mrlm: 2
gcmz: hvcj * bgcq
msbn: 2
jqjs: 2
rpfz: tnsf * jqjs
zwrg: 2
gtsm: 2
cdmj: 2
tqdn: mrsc / qjrn
dqzg: 4
gbfh: ntjd + ddwn
cgtq: 15
qwhs: 3
fnnp: 3
zzdm: bpzm + sbvw
wgms: nwsm * cqts
ngfw: zdgb * bcjq
gsfq: 2
ljfh: chtc * zswz
hvmd: 2
cgdv: jsfd * njhn
hdvl: 1
zcgd: gpgw * jfwm
rpcg: zbqv / lpjc
hmnw: 4
pqgd: rfzr * ljzm
bdzw: jdlz * gmmd
cjws: 4
ggjm: gwrf - glrj
jlzl: znff + wvms
rwsp: 20
ntjl: 2
brmd: 17
jlhs: 2
jwjv: wtsr - jqfb
qjdm: 12
sbsp: 5
qhwv: 2
pwzl: qqhc * cfvt
ljsc: hqpn - nffr
sqtn: msmd * tsdh
fdhj: pnwq * zrmw
lgcw: 3
nqzf: 5
jcdw: rvjp + cpbq
lvtt: bpgv + vbww
lqtn: jlqz * cthz
lcsh: 2
pnbv: hqqr + jjlf
ccrw: 2
lnmc: 17
gmhm: bcqw * jpvm
mvlt: vqzv + zfwd
hfcj: 5
rjlr: 5
pqmm: 2
mfsl: 19
qcwd: 5
wqdj: trrf - bpwb
zswz: 2
tvzz: rhnn + mmwg
vltd: lfvz * fgvr
tcvp: 7
gnfw: qgsd / zjhg
ccjl: 6
tqhl: dbwr * zjhc
lwcl: 5
zvmr: nqvj - bcmh
fcct: jhzp * mdtr
brnb: 2
mqqc: jswg - jfzf
ldnl: wchq * gvch
mdtz: 13
spnm: 2
bszh: sprc + mndl
wchn: 11
cfrq: cdlj / wbwv
rttf: 4
qcbd: jrjs + cwjc
dvjs: mjtt * zbjc
nsvc: tbbn + cjmp
ffvj: 5
qzvz: 9
tvqf: njss + jfth
zscf: 3
gbfq: 5
jgts: 19
qsvs: qqfz + bsjz
vjts: hfbh * jplp
sndt: nthl * vrbd
rbdb: nfdq * cnzd
vwqc: 2
hnfm: rprr * djdq
mtnn: vqmn + twbw
rbnq: rhqv + bgsn
zvtr: jvql * zpsf
sqqj: 2
jmwn: rshh + smdc
dgzw: 11
bcbg: mqsq * vhtr
zrzd: 7
zhlm: 3
smlf: zfft / bsnc
jzps: 1
wddr: 3
gzsg: 2
glpz: 13
mgwp: tfct * cfcz
cgbj: gfcz + pddn
qvtr: vqzr + vlzp
hssf: 2
zsqf: bvnf * lpwv
mmqb: 4
vbmq: rnbg + jtnm
vtgz: rscp * glwd
phzq: 2
qmwh: rzfm - jcqs
vsjf: lqzn * dfts
ppld: 9
djdq: 2
rwcc: tqdn * clfs
jmfb: ftdf * mhnw
nlds: dmrn + fdpd
gsvd: 2
bsnc: stgq * jdgf
hgdg: dgpj + bcqh
cplg: 5
mwgw: 3
pmsh: czdp + fmcn
dwqv: 2
nlsw: glcs * lbmv
fzfj: 5
rjls: 7
pgjv: 4
tbls: bqvb * cdqm
dvnt: 8
mpbh: lfms * lwbb
fnjr: svhs / wpjt
wvfb: 5
czzf: 2
grhg: 5
shtc: 4
dsbn: ngzj * mtwf
lqvh: gcsm + vvdl
srsv: nvzd + hqsn
vfbz: zpbs + zbwd
bmhz: qmwh * jfbd
qdbn: vsjd * vrjz
hjzz: vbgh + vzfr
tnmb: rttf + nhvg
sglg: pddl + cqzp
jmzs: 2
bpfh: jbfw + hgvc
dqbt: 2
tcpl: 2
vldr: hbdh * cplp
pgpw: 7
lrhz: fsjt * dgcw
msvq: 2
vddt: 8
jmms: zwld / qcfr
pssw: rdrm * jpnq
cfqh: pfrd + wtcf
mbhm: hlds * fvtb
lsqt: 5
dfcj: 2
bslv: 2
hwrp: lgcg + nrbd
jnbm: 9
sdwf: hpgz - rgcl
hqws: qptv + sfrh
rgqp: vbct + vvsm
wcdc: qrrb * rddc
rwld: 5
slhg: jzbd * drfz
vlsv: tgbz + gswz
zpbb: frsz * wwvq
thfw: lbsr * jngq
vjfr: 2
pvgg: 2
wrlg: 2
vdzm: 8
rjfn: jmdd / sfvl
jnfn: rgqp * lrdv
ndqd: rhdt * tlgj
lqdb: 15
bpgs: jmwn * wzbd
zhlj: qlln * qsrp
jhmj: 8
cmcq: hggq * rbgr
cntn: 4
qtgm: fqhn / mhhh
lrwv: dbzw + cggg
zcdn: brpd * hfsv
mdlr: qwjg * fqwr
swwt: 4
hnft: 5
pbzh: qdzl + thrd
bbzv: sglg * cdtt
qjrn: 2
jzmj: vnhg * nlsz
zqcs: 3
mgzg: jlrd - qgws
dwlh: pgnm * rcnh
vhjt: tllw - rbjv
wffd: vdhn * mlpn
tbsp: nlvz * bfjp
jtbv: 9
bqmw: 5
qjqv: wvfb * sbft
srmb: 3
bnjh: 5
ntqd: 7
grnm: zvpc - bqsj
cfjj: tlfc / tdpj
vgrb: 2
stgq: 4
tbfv: lhjq + ztfb
mhlg: 5
tmzn: 2
sbft: cfcq + dwlh
qpqj: 7
tvpv: nhbs * sfzm
njwf: 5
mvtt: 1
gltf: bvgq * slrv
bbzs: 11
ppbl: 8
qpsj: fcml + ngrb
tztt: 3
gnln: cjqt * rqmt
mmrq: fhsf * gnds
qqfm: dzsd + qpcr
cjjl: 4
tlbs: 5
lzdr: nncb + cmnr
hflq: pbrv * wzld
blqv: 3
mjtt: pvln * hsgw
vdnz: 4
qjbz: 4
bbcr: mvzt + mztb
lzzr: 5
rwcs: 2
crvh: nnwd + ptsl
bbqz: dgpv * vncg
dbmw: 2
rqqb: fthv * cdtw
vbct: jfpn + glhb
wfcb: 3
hqsd: 3
zzjq: 2
mpml: 11
rbzs: 2
hqzz: 4
zjpv: jglh * qctt
jswg: gsvh * hfzd
ddwb: 1
mcqz: 5
pnfs: jzcz + mwgv
dpfb: 2
sgzl: 6
tgch: 5
twss: fsrq * jtpc
wbbm: 3
jglh: 2
cslw: 3
vqzv: spvs + hdnr
tncb: fbbs + lfwn
szch: jljv * tdhm
hzcv: cjdf + fwbz
rmhv: 6
tllw: vsjf + gvln
fdbn: wmwc * pmvb
gthr: vjzg - rtww
sjbv: msqh * bphh
cpgm: 5
bqgc: btzj * vpqg
cdrq: 2
wstv: gnfw - qdfr
qwqg: 3
cdsv: 6
zmdn: 3
wtwl: 5
dcwz: cnlz * mnrf
cmhw: tmrl * rvsl
rjhw: wllc * hzwc
tqbz: wccq / sntd
qtqz: 5
hvcj: fgzq * hnfm
sczv: djlc * smjt
qzlt: 2
gbhr: 5
bdtw: jgdt + qnjv
nhmw: swzh * fbvt
nvbr: zdld * dznv
ccld: 4
mdrv: 2
dhrs: 4
vrcm: 7
qdtb: hhzv + mvcr
wmzc: pcrm * hmnw
rvsl: 2
snwv: 2
jlrd: bwcw * jlwj
nwnm: 16
qsnm: 5
srdr: 3
ddzt: ssgd * ntvz
wtbd: wdfv + nhvr
wzms: zjhw * jhgr
humn: 2326
cpfv: qtnb * sjpj
hmrg: fwwd + qcbr
bvgq: 2
ngzj: 3
hvvh: 3
glhb: 4
hbmg: 17
hltq: gnnh + btlz
hpws: 5
sttb: bfzz + nzbq
nvwn: 3
lscd: gvsh * fshp
mjwf: 17
shhn: cwdr * jcdw
mdwn: 2
zplv: 10
ddjs: 2
hdgr: rcjj * bhzb
ccps: 2
msmd: tzbf * qcrb
tqhv: ttvd + cqft
wrmc: hzqg * vhqr
ttll: wtms * tptf
root: ddzt + rmtp
mjws: rwcc * vmcn
lhzl: 4
srhl: nwfr + spfw
zvpc: trgl * cvcs
fntz: 2
smrs: 3
gzvs: pfcc * gbbd
vbcs: jdgl + jdbg
ffcp: 5
wcdp: fjmh + mmqp
mpbq: rnsg * psws
qrbn: 5
bwvw: dfcf * ldqw
pcvh: qtqg * gfnj
pjbj: 2
lrdv: 5
bmdw: jpgj + wtbd
bqcp: ngqh * cplg
szzj: jbtb * bpqn
znpq: 12
fwwd: 4
svdv: jpqg + btzc
nvjv: qlgg / jlhs
blvq: 3
fbvt: hprp + llhg
fqgl: wcsj + rrwn
bmgd: 4
cqrf: 20
lmfr: 2
vmcn: hqws + pndz
vzbr: 7
mclc: qsnm * mwrq
fhfj: nqlt + thmv
dbvd: 2
ntvz: glpp - jjdn
rqmt: 3
dwhz: 6
zjhc: 3
hfww: fzsv + lzdr
btqd: 2
pchp: bhvq + vlll
jpql: znqm * cchd
zmzr: 14
jcqs: njcr * nfzl
rsmb: 2
lbhb: hpld * vwzl
wzbd: swrf - wznr
nmbd: 2
dlpb: nsjd + tdvr
lbqs: mncg * qdwb
mzcm: lnrw + dfcd
cdjv: 2
rhfc: 14
hssr: ngfw + cnjh
zwdn: 5
ntms: 17
szlc: 2
bgrv: mlqg + sndt
vbww: jcjl * mqjw
fhrl: 2
jzzn: 4
qrdl: 8
cvrw: 2
hlfv: 2
lnjg: cbnw * mwgw
hvzv: 3
bpfq: 8
mbdh: 18
nbtd: twds - szch
vbqj: mlcp * bsvz
ldrg: gjrc * rlzn
lfmf: htrt * qvrv
sclt: 3
wjvv: hqzc * pvdt
wfhf: 3
hcrv: 4
sbwj: npph * pgtv
fbvc: swsp * qhnn
srlq: lnjt - phhm
bftl: 4
cfwj: nnjj * qsdt
qjzc: 15
wczs: hltq * hgsf
bsdm: 2
mfwv: lphf * jnvd
jmpl: ndhg - cgzc
wgjl: 2
mvwt: tfdz + mwnr
nmnt: vrhn + nvbr
jcgg: tlhc * ndnm
crsm: lrcp / tzqp
qqdf: jbjs + gjmm
gfjj: dsbn * bhqg
wbvn: 2
twds: dnhv * gswg
hjhs: vstm - gtrf
rjpd: jlgr - fwcr
ftdf: 17
rqpp: ghfq * ntld
mfzf: zcrb + wmst
nnbg: wjvv / zwpw
trrh: 5
gsll: 10
qtmd: dbpb * gqth
sjnt: 7
cwzp: 2
dgcl: rhfc / njfq
jjfd: pfgz - szjr
ptrg: dwrm + gppg
sfbt: 3
cwjc: 5
frhm: 3
mngb: wqbj + bmml
glzz: jgwl + dgsc
hgwq: 3
gtmc: 2
zrmw: 4
cwvf: zqgq * cshz
gpdn: gldm / twzb
qlln: lrnd + tzmz
glgl: 5
frhc: 2
bjtd: 8
jlmd: 3
jzdb: 19
lbsr: 3
grtm: gzvs + hcsj
swrf: zcsp + mjbn
jprl: bblf - ncwg
rrtt: 2
mtfv: ntms * smzz
fqsc: pffb * znvl
lbzr: dgvd + cqzs
gdqj: ljsc * vgrb
ppjd: zhlm * rmmp
vdhn: 2
nzwr: nsjh * jnmz
cjdf: 5
qzrq: msmh * sjbh
mnpm: qmfg * vsjm
bqml: 2
rrjh: tpcp * chsm
tpft: 11
wngs: rhrt * jsvp
mzgg: wtld * sbmq
bdpj: lmwj * cntz
crwb: srsv / wfcb
wwhg: 2
hzvl: 10
pnqq: 8
ftps: 2
wwrc: 2
rmft: 11
bglm: gfqd * zwfr
pblm: 5
tdrf: 1
bcpc: jdpv / gngz
gqwj: 10
zrjs: fftt + mrfh
dqzn: 6
rcwl: zcqm + bqgc
plmq: crsm + jgql
wgfm: zqdv * sjpv
pqvf: 2
gwhl: nctn * lzds
rrqf: 2
jvfs: 2
fcdc: 6
zlwc: 3
gsrs: 2
vcvd: 1
gfnj: shtt + tzzt
pcrw: 1
lqzn: wpdh + nqlm
bvjz: nmrd * splg
qjjw: pvfc + gvlq
rscp: 6
thtm: 5
cjmp: 4
gvlq: lqvh + fwgn
pvmw: mpml * btqd
qmnw: bjhg + mzcm
ggcv: 4
vbfq: 4
fsrz: 15
cgcg: 2
cmsc: qvfr * wmql
vpjp: ntpv + hvpn
twbw: qgpd + cdjj
cssd: 4
vshf: qjzc + sbzb
cznm: bvbw * ngdh
ppzl: 4
fptz: wvrz + qwpc
bnwp: 4
lfrh: 11
wcgg: pzqp * wzvw
mvht: nfqn * dgrb
zwmw: 9
dhzh: jtvb * cqjc
jdps: jbww + zdwh
fnmq: 3
glrj: 4
qbhd: cfgm / lhzf
rlql: szwr + zwbh
wbwv: 2
nfws: 3
nfml: hrvn + bntn
qwhj: 3
mbbt: 17
rdrm: 3
twdp: qwhj + qmbr
qgpg: prvs + fltp
bdtf: qlqt * rzzg
cpzm: 5
qjcl: cmsc + bhfj
pflw: wcdp * sjlq
ctmj: gmfn + qmnw
dbwr: 9
pnpt: 8
mlcp: 6
tzvd: nbtd * cfnn
twfp: dhzh / nnnl
wmpt: lbgc + bftl
zzzd: lgcw * wsfp
jttc: 4
gwlv: 7
fhsf: 3
jspb: 12
dzsd: 5
hphz: 13
qtgf: 2
nvzt: 2
hdzp: ctwp + wbbm
plch: rlsq - rtrf
lphj: htmw * jgwh
cdrh: tcgz + zbns
gmlb: dmjp + nzhn
wzld: 2
twmg: 3
lmpd: dndn + rmnb
qqfz: qtmd + jlgs
vnwz: 17
sllm: 2
vqnc: 14
hclm: tghn + zwhs
wvdd: 6
hgjl: 4
lmmc: 5
ltct: mnjp + nsvc
thcs: 2
cclp: jqpl + fhfq
mbbg: wqwr * ppjd
lmwp: 8
vjfs: cdrq * jwwl
jfzf: drhj * crwb
ntbd: nsbb + lhsm
bzdl: qfrp / wfmq
fldt: cslt * wfhf
wstr: tdsq * vbpr
bsbl: sdmf / wbvn
svch: 4
lqcl: 4
ljmh: svqp - spvn
sdpm: zwrg * jmrl
vwzl: 20
gchc: 8
jpvl: 2
bldj: 4
zfwd: lgmz * zvsn
lbfm: bbcw * vrdz
frzh: 9
hsdf: 5
hpsr: ppwb / hvzv
tbbq: 7
bdtv: cprc + tnjr
mhzv: zspc - dltj
vzmt: 3
lpjc: 2
dgrb: 4
cmbv: 4
rwmp: rpfz / pqvf
spvw: gbbh * zzgf
lnjt: qcsf - mnmm
vrdz: wrtc * crrs
tvtv: 10
mhhh: 5
pdsn: 17
jdgn: 2
vncg: 3
dfhc: 8
qrwc: fnlc / zghq
bnvw: tgbm + tplg
lmvn: grzl + dhwf
zvvd: ltsq * tnjw
jhlw: 8
mstp: nrjc + tqst
wwlw: mpfq * lsqt
nvfm: sqqj + lmqh
grwd: 1
stcg: dwmd * vrqw
cwnf: jdnp * nfbj
csvz: gvgc * trfl
mhbm: fgnr * tqcp
hqqr: wjgp * lfwg
dhjb: 5
dtjj: grnm / dfvt
gldm: gflv * clfz
mlrj: 16
gqwd: gnvm * fjgw
bbcw: 2
qrtl: 4
cpcg: lbhb + ppcb
gmvf: pjhh * jrlq
pdlb: stcg + mnlc
qdzl: ffls * dwdz
mpgd: rjpd + jbmc
zcjn: srmb + zszq
wphz: 2
rdrz: nfwp * wzzm
hdsw: sdhm * bhqj
hssj: 9
sjpv: 2
jwrw: 9
cccs: lmpd * tsfc
hhth: 2
mgdj: 9
ndrm: 7
pqbt: qgww * qjbq
sdtq: nccg * pjbj
hplg: gtsm * wctw
rcbg: svbm + mvlt
tbcs: 2
sdgd: ccld * gwrz
lszb: qcns + lmlq
hntv: 2
mlvs: 5
tmsh: 7
lrsl: rbzs * gpsn
lfhj: zvff * fnjr
bcqj: smrm * hgwq
dfvt: 2
lnth: mvrd * zldg
tthh: mhlg * fwhf
ntld: phzq + wmbn
hrzt: bmcl / hssj
pcrm: hnfc - ljzj
qdcz: hjcf / cvhs
dfcf: 3
mqfd: 8
psws: 4
rbzt: 2
slzc: wzms * wpwh
svrq: bjtd + qfcc
fbbs: smgs * gbhr
wgsd: jjzg * hpvm
sdlw: 13
cblg: wmlh + zwsf
bmml: mccv * vjcj
btfv: bfhl + cgjq
glpp: wgcl * rtcg
dmts: sjnt + cmdv
svtt: 4
vnfm: fshr + zhqm
tfgn: mpql + msvq
qzqb: rcbg / tnmb
lpcq: 12
twdw: rsbs * mgsr
jbpv: sjrb * qndt
sggd: zzhd * vnth
vjzq: 6
mfvv: 11
fsgm: rtvv + schn
nfch: mtnn + qcbd
trmr: 3
czzv: jnfn + sjdr
vmbq: scqd * qtgr
qcbr: 10
rnsg: 3
svvr: smrs * jhgh
mjmr: 14
vnbp: btrt * rtrr
zcvq: 14
tptf: 5
swsr: jwjv * dnlr
mndl: 8
tbvc: 4
hbdh: fvsz + qtqz
bhmt: 5
qqfq: mfwv / stdg
vsbb: fwfq * zbtc
pvpw: zcvq + zttf
bvbw: 2
vlnn: 6
pwjt: tbjs * lzfd
vhqr: 4
qcsf: jqcg * tthh
hzjg: 2
jzpz: zrsr / cjws
pncr: hrrt / bggm
mnmm: dcwn * spvf
qptv: pjsq * nctc
msqh: zlfg * qldf
zzzj: pnvp + vtcn
sbvw: qdbl + brmd
gvgc: qsvs + mlrj
gvlc: llpj + tfvs
vccz: mrzz + cqnz
sdmf: wqdf * mcnw
jdlz: qzpv + vbrr
qrdj: tdlt * bqnf
zzfb: 2
psjj: 5
pjhb: rbhj * lpcq
pmlb: 5
nsjh: qpbt + rjjd
gnnh: wgzs * csmq
cjrt: zjpl + bglm
mvds: ftps * zvwq
wmbn: 5
tvvf: ffzh / fntz
vgcv: 11
mnlc: rrbz * lmmc
gfgw: lbfm + qjfg
gwvc: hqfm + tcfl
jvpg: 2
gbcd: jbmb * wznh
sbws: ngzn + vggc
ppjm: vltd + pvmw
grpr: lrsl * vwcw
jfwb: mvll + lzzv
tflf: 6
bfhg: nsgj + sqqb
tqst: shlf + vdcr
jswq: blhr - qdqz
ztjh: 2
hmfw: 4
gtmv: 2
wvrz: qtsz * gjwg
rzmf: 2
lzqv: 13
bntn: lnff * ghfb
bpsv: pcdm * rdls
sfrh: jcwg * lrbl
cmtc: wgsd + ncpp
gdtf: wjrw * qcwd
djnz: 3
chtr: 2
lcjw: hpsr + zvlw
fzbq: 5
vvrr: 3
vdhl: 11
zmcd: swvg + trvl
gnqd: 4
rwgd: 3
fgzq: 3
gjnw: gltf + dtqg
vfgm: 5
pcls: sdlw * cjpj
tnsf: mhgl + sgsn
ggzq: 6
ddqw: rbnq * lzsn
qgfg: 6
rmmp: 5
hthc: 9
tfgl: vrcn * rdws
hlds: 2
jsqn: vcvc + ffhd
qwcm: 2
whfc: 1
lgcr: 2
mhfl: gsll * rhns
nftf: cmzw - ddwb
mrzz: mqmp + rsvf
lzsr: wchn + fvhw
sgpt: mzfg + vmhj
ghfb: mrmp + prmt
mbrv: rvsr * lzsr
zbmj: 10
llrd: trjn * zvjm
svqp: bhnl * pmsg
mcnq: 20
mcqc: 5
zjgr: 2
wjtn: 2
wccq: dhvv * hzcv
ttzz: 4
vrnf: tdjq * mvnc
vlll: fhqw * wwlw
vrfj: 3
nvvr: 4
vbvm: 5
ltbf: 2
mfmt: znfv + tflf
btqz: 20
pnhf: 1
tbjs: 5
nhpd: bhmt + cmtt
gjmm: 2
wqgj: lmbg - jlng
vfvh: phsw - hpws
vjcj: 5
glhl: 3
lftw: 2
tdcd: crtq * lwcl
gjsd: fbnf + fznd
rjhf: srdr * pqcr
ppbn: bhcq * prlt
djtg: qfhm - zqpg
qlsn: 1
hrjt: lftw * mqfd
wccs: ggzq * tldv
wvlz: cfwj * chwr
fmtz: pcfr * rgvn
wpwh: 2
vfhr: lrhz / vdpd
lgwv: vfhr - wgfm
jswt: hsrq * fmjc
pndz: nfws * vdqr
szjz: jprh + fqsc
hlzs: 2
pmgv: lbqc + nnhn
rgpm: 2
jdgf: 2
jzsz: 2
dfnh: 18
vqgc: sbws * cgcp
ghfq: dnhh / rrtt
dpdq: dmts * rtqf
hght: jbln + zftz
zfmj: zcjp * bghz
pcfw: vrfj * sclt
bcdj: 3
htlp: 3
fmjv: 11
jrjc: cgbj + dvnw
fwvn: jnnn + hwhv
qrqf: szzj + vshf
wpmq: bbzj * qrwc
hfjb: mflq * djgt
zspc: psmr * zznq
gfgf: bnjn * wwcg
vrhz: 2
rtfr: 2
vdqr: wbsh * jjvh
dcwn: 11
jwtt: ppbn / tfzq
cgcv: jmfb + nfml
cgmt: rcll * jmht
wlrt: bcvc + lnmc
nrqj: 1
vmdl: 4
ncrl: wvjn + mvhm
ctwp: smbz * vrhz
pdnq: bbvq * plfp
qhmc: 1
zbtc: rwpr + rgzp
lfrs: 2
wmsq: 2
hfbh: qcpl * qwfp
rdrc: zwrd * gmjh
hnrr: 5
qwll: 14
wsqv: 5
vdml: pdtm + zwsd
hgrg: gnhf + mgwp
bbzj: 2
bzvl: 3
lhgt: qhwv * cbmt
jbww: 16
nwhd: 2
bdrs: 13
wwcg: 2
dsdj: hjhs - mcgt
stpl: nnpl + nrqj
mjbn: flnn * hgdg";