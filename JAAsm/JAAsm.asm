; SHA256Asm.asm
.code
; Export functions
PUBLIC Sigma0Asm
PUBLIC Sigma1Asm
PUBLIC BigSigma0Asm
PUBLIC BigSigma1Asm
PUBLIC ChAsm
PUBLIC MajAsm
PUBLIC ROTRAsm

; uint ROTR(uint x, int n)
ROTRAsm PROC
    mov eax, ecx    ; first parameter (x)
    mov cl, dl      ; second parameter (n)
    ror eax, cl
    ret
ROTRAsm ENDP

; uint Sigma0Asm(uint x)
Sigma0Asm PROC
    mov eax, ecx    ; input parameter
    mov r10d, eax
    ror eax, 7
    ror r10d, 18
    shr ecx, 3
    xor eax, r10d
    xor eax, ecx
    ret
Sigma0Asm ENDP

; uint Sigma1Asm(uint x)
Sigma1Asm PROC
    mov eax, ecx    ; input parameter
    mov r10d, eax
    ror eax, 17
    ror r10d, 19
    shr ecx, 10
    xor eax, r10d
    xor eax, ecx
    ret
Sigma1Asm ENDP

; uint BigSigma0Asm(uint x)
BigSigma0Asm PROC
    mov eax, ecx    ; input parameter
    mov r10d, eax
    ror eax, 2
    ror r10d, 13
    xor eax, r10d
    ror r10d, 22
    xor eax, r10d
    ret
BigSigma0Asm ENDP

; uint BigSigma1Asm(uint x)
BigSigma1Asm PROC
    mov eax, ecx    ; input parameter
    mov r10d, eax
    ror eax, 6
    ror r10d, 11
    xor eax, r10d
    ror r10d, 25
    xor eax, r10d
    ret
BigSigma1Asm ENDP

; uint Ch(uint x, uint y, uint z)
ChAsm PROC
    mov eax, ecx    ; x
    mov r10d, edx   ; y
    mov r11d, r8d   ; z
    mov r9d, eax
    xor r9d, r10d
    and r9d, r11d
    and eax, r10d
    xor eax, r9d
    ret
ChAsm ENDP

; uint Maj(uint x, uint y, uint z)
MajAsm PROC
    mov eax, ecx    ; x
    mov r10d, edx   ; y
    mov r11d, r8d   ; z
    mov r9d, eax
    or r9d, r10d
    and r9d, r11d
    and eax, r10d
    or eax, r9d
    ret
MajAsm ENDP

end