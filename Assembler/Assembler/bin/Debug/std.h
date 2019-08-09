//missing HLT opcode
#macro hlt {
    hlt_label:
    jmp hlt_label
}

//missing NOP opcode
#macro nop {
    add a 0
}

//define string
#macro db_s value address {
    //mapping value into str array
    #map value str
    #sumdef str.length address
    #fordef iterator1 address str.length 1
    mov b, str[iterator1]
    st iterator1
    #endfor
    
}

//define byte
#macro db value address {
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
    in b
    st iterator2
    #endfor
    #undef leng_in
}