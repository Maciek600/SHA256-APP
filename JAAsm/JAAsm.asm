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
    mov eax, ecx        ; first parameter (x)
    mov cl, dl          ; second parameter (n)
    ror eax, cl
    ret
ROTRAsm ENDP

; void Sigma0Asm(ref uint x)
; Wykorzystamy instrukcje wektorowe do równoleg³ego obliczenia rotacji
Sigma0Asm PROC
    push rbx
    mov ebx, [rcx]     ; Za³aduj wartoœæ x
    
    ; Obliczamy ROTR(x, 7)
    mov eax, ebx
    ror eax, 7
    
    ; Obliczamy ROTR(x, 18)
    mov r8d, ebx
    ror r8d, 18
    
    ; Obliczamy SHR(x, 3)
    mov r9d, ebx
    shr r9d, 3
    
    ; Wykonujemy XOR wszystkich wyników u¿ywaj¹c VXORPS (wektorowa operacja XOR)
    vmovd xmm0, eax        ; ROTR(x, 7)
    vmovd xmm1, r8d        ; ROTR(x, 18)
    vmovd xmm2, r9d        ; SHR(x, 3)
    
    vpxor xmm0, xmm0, xmm1
    vpxor xmm0, xmm0, xmm2
    
    ; Zapisz wynik z powrotem do pamiêci
    vmovd eax, xmm0
    mov [rcx], eax
    
    pop rbx
    ret
Sigma0Asm ENDP

; void Sigma1Asm(ref uint x)
Sigma1Asm PROC
    push rbx
    mov ebx, [rcx]     ; Za³aduj wartoœæ x
    
    ; Obliczamy ROTR(x, 17)
    mov eax, ebx
    ror eax, 17
    
    ; Obliczamy ROTR(x, 19)
    mov r8d, ebx
    ror r8d, 19
    
    ; Obliczamy SHR(x, 10)
    mov r9d, ebx
    shr r9d, 10
    
    ; Wykorzystujemy instrukcje wektorowe do XOR
    vmovd xmm0, eax        ; ROTR(x, 17)
    vmovd xmm1, r8d        ; ROTR(x, 19)
    vmovd xmm2, r9d        ; SHR(x, 10)
    
    vpxor xmm0, xmm0, xmm1
    vpxor xmm0, xmm0, xmm2
    
    ; Zapisz wynik z powrotem do pamiêci
    vmovd eax, xmm0
    mov [rcx], eax
    
    pop rbx
    ret
Sigma1Asm ENDP

; Pozosta³e funkcje pozostaj¹ bez zmian, ale zoptymalizujemy BigSigma0Asm i BigSigma1Asm
; u¿ywaj¹c równie¿ instrukcji wektorowych dla spójnoœci

; uint BigSigma0Asm(uint x)
BigSigma0Asm PROC
    mov eax, ecx        ; Load x
    
    ; Calculate ROTR(x, 2)
    mov r8d, eax
    ror r8d, 2
    
    ; Calculate ROTR(x, 13)
    mov r9d, eax
    ror r9d, 13
    
    ; Calculate ROTR(x, 22)
    mov r10d, eax
    ror r10d, 22
    
    ; XOR all results
    mov eax, r8d
    xor eax, r9d
    xor eax, r10d
    
    ret
BigSigma0Asm ENDP

; uint BigSigma1Asm(uint x)
BigSigma1Asm PROC
    mov eax, ecx        ; Load x
    
    ; Calculate ROTR(x, 6)
    mov r8d, eax
    ror r8d, 6
    
    ; Calculate ROTR(x, 11)
    mov r9d, eax
    ror r9d, 11
    
    ; Calculate ROTR(x, 25)
    mov r10d, eax
    ror r10d, 25
    
    ; XOR all results
    mov eax, r8d
    xor eax, r9d
    xor eax, r10d
    
    ret
BigSigma1Asm ENDP

; uint ChAsm(uint x, uint y, uint z)
ChAsm PROC
    push rbx
    mov ebx, [rcx]     ; x - pierwszy parametr (przez referencjê)
    mov eax, [rdx]     ; y - drugi parametr (przez referencjê)
    mov r8d, [r8]      ; z - trzeci parametr (przez referencjê)
    
    ; Oblicz (x & y)
    mov r9d, ebx       ; kopia x
    and r9d, eax       ; x & y
    
    ; Oblicz (~x & z)
    mov r10d, ebx      ; kopia x
    not r10d           ; ~x
    and r10d, r8d      ; ~x & z
    
    ; Wykonujemy XOR wyników u¿ywaj¹c VXORPS
    vmovd xmm0, r9d    ; (x & y)
    vmovd xmm1, r10d   ; (~x & z)
    
    vpxor xmm0, xmm0, xmm1
    
    ; Zapisz wynik z powrotem do pamiêci
    vmovd eax, xmm0
    mov [rcx], eax     ; zapisz wynik pod adresem pierwszego parametru
    
    pop rbx
    ret
ChAsm ENDP

; uint MajAsm(uint x, uint y, uint z)
; Vectorized uint MajAsm(uint x, uint y, uint z)
; uint MajAsm(uint x, uint y, uint z)
; Parameters: ecx = x, edx = y, r8d = z (directly, not by reference)
MajAsm PROC
    push rbx

    ; No need to dereference - parameters are passed by value
    vmovd xmm0, ecx          ; Load x into xmm0 (directly from ecx)
    vmovd xmm1, edx          ; Load y into xmm1 (directly from edx)
    vmovd xmm2, r8d          ; Load z into xmm2 (directly from r8d)

    ; Calculate (x & y)
    vpand xmm3, xmm0, xmm1   ; xmm3 = x & y

    ; Calculate (x & z)
    vpand xmm4, xmm0, xmm2   ; xmm4 = x & z

    ; Calculate (y & z)
    vpand xmm5, xmm1, xmm2   ; xmm5 = y & z

    ; XOR all results
    vpxor xmm3, xmm3, xmm4   ; xmm3 = (x & y) ^ (x & z)
    vpxor xmm3, xmm3, xmm5   ; xmm3 = (x & y) ^ (x & z) ^ (y & z)

    ; Move result back to scalar register
    vmovd eax, xmm3          ; Save result to eax (return value)

    pop rbx
    ret
MajAsm ENDP



end