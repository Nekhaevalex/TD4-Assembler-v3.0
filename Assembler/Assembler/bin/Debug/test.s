#import std.h
#import io.h
#pext alu.pext 15

;;; SLOWクロックで約1分たったら、出力ポートを1111にする。
;
MAIN:
OUT 0      ; 0を出力ポートに送る
ADD A 1    ; レジスタAに1を加える
JNC 0      ; 桁があふれなければアドレス0にジャンプする
MOV A 11   ; レジスタAを11にする
ADD A 1    ; レジスタAに1を加える
JNC 4      ; 桁があふれなければアドレス4にジャンプする
OUT 15     ; 15を出力ポートに送る
JMP 7      ; 停止する
set_io_on 14
mode_read
enable_auto_inc
bus_on 1
in a
hlt