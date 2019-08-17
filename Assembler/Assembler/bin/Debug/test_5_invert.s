//输出input的取反
MOV A, 0xF
IN B
in:
ADD A, 0x1
ADD B, 0x1
JNC in
MOV B, A
l7:
OUT B
JMP l7
