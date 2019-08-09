//Compile in 4-bit mode without optimization!!!
#import trace_debug.h
generate_empty 17
label1:
add a, 10
generate_empty 32
label2:
add a, 10
generate_empty 5
jmp label1
generate_empty 37
label3:
mov a, b
jnc label2
add a, 2
add a, 2
add a, 2
jnc label3
generate_empty 10