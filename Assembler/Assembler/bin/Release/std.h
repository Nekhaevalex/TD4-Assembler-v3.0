//missing NOP opcode
#macro nop {
    add a 0
}

//missing HLT opcode
#macro hlt {
    nop
hlt_label:
    jmp hlt_label
}

#macro gqt operand label {
    #define addit 255
    #resdef addit operand
    add a, addit
    jnc label
    #undef full
    #undef addit
}

//define string
#macro db_s value address len_name {
    //mapping value into str array
    #map value str
    #define store_to address
    #fordef iterator1 0 str.length 1
    mov b, str[iterator1]
    #sumdef store_to 1
    st store_to
    #undef str[iterator1]
    #endfor
    #define len_name str.length
    #undef str.length
    #undef store_to
}

//define byte
#macro dbx value address {
    mov b, value
    st address
}

#macro out_addr address length {
    #define leng_out length
    #sumdef leng_out address
    #fordef iterator1 address leng_out 1
    ld iterator1
    out b
    #endfor
    #undef leng_out
}

#macro in_addr address length {
    #define leng_in length
    #sumdef leng_in address
    #fordef iterator2 address leng_in 1
    mov b, 0
iterator2:
    in b
    mov a, b
    add a, 255
    jnc iterator2
    st iterator2
    #endfor
    #undef leng_in
}