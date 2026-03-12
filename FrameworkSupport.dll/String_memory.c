Source file: Microsoft.FailoverClusters.FrameworkSupport
Generated time: 20260130_192928
==========

FUNCTION: ??3@YAXPEAX@Z @ 0x1800017E4
----------
// attributes: thunk
void __cdecl operator delete(void *Block)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  free(Block);
}


==========

FUNCTION: ??3@YAXPEAX_K@Z @ 0x180001BF4
----------
// attributes: thunk
void __cdecl operator delete(void *Block)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  ??3@YAXPEAX@Z(Block);
}


==========

FUNCTION: free @ 0x180002524
----------
// attributes: thunk
void __cdecl free(void *Block)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  __imp_free(Block);
}


==========

FUNCTION: memset_0 @ 0x180002548
----------
// attributes: thunk
void *__cdecl memset_0(void *a1, int Val, size_t Size)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  return memset(a1, Val, Size);
}


==========

FUNCTION: ??2@YAPEAX_K@Z @ 0x1800025EC
----------
void *__fastcall operator new(size_t Size)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  for ( i = Size; ; Size = i )
  {
    result = o_malloc_0(Size);
    if ( result )
      break;
    if ( !(unsigned int)o__callnewh_0(i) )
    {
      if ( i != -1i64 )
        __scrt_throw_std_bad_alloc();
      __scrt_throw_std_bad_array_new_length();
    }
  }
  return result;
}


==========

FUNCTION: _vsnwprintf @ 0x18000268C
----------
int __cdecl vsnwprintf(wchar_t *Buffer, size_t BufferCount, const wchar_t *Format, va_list Args)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v8 = _local_stdio_printf_options();
  result = o___stdio_common_vswprintf_0(*v8 | 1, Buffer, BufferCount, Format, 0i64, Args);
  if ( result /*signed*/< 0 )
    result = -1;
  return result;
}


==========

FUNCTION: printf @ 0x1800026F4
----------
int printf(const char *const Format, ...)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  va_start(va, Format);
  v1 = o___acrt_iob_func_0(1u);
  v2 = _local_stdio_printf_options();
  return o___stdio_common_vfprintf_0(*v2, v1, Format, 0i64, va);
}


==========

FUNCTION: ?StringCchCatW@@YAJPEA_W_KPEB_W@Z @ 0x180007354
----------
HRESULT __fastcall StringCchCatW(wchar_t *a1, size_t a2, const wchar_t *a3, size_t a4)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  pcchDestLength = 0i64;
  result = StringValidateDestAndLengthW(a1, a2, &pcchDestLength, a4);
  if ( result /*signed*/>= 0 )
    result = StringCopyWorkerW((STRSAFE_LPWSTR)(v7 + 2 * pcchDestLength), 260 - pcchDestLength, v6, a3, 0x7FFFFFFEui64);
  return result;
}


==========

FUNCTION: ?StringCchCopyW@@YAJPEA_W_KPEB_W@Z @ 0x1800073A8
----------
HRESULT __fastcall StringCchCopyW(wchar_t *a1, size_t a2, const wchar_t *a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  if ( a2 - 1 <= 0x7FFFFFFE )
    return StringCopyWorkerW(a1, a2, (size_t *)a3, a3, 0x7FFFFFFEui64);
  result = -2147024809;
  if ( a2 )
    *a1 = 0;
  return result;
}


==========

FUNCTION: ?StringCchPrintfW@@YAJPEA_W_KPEB_WZZ @ 0x1800073E8
----------
HRESULT StringCchPrintfW(wchar_t *a1, size_t a2, const wchar_t *a3, ...)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  va_start(va, a3);
  if ( a2 - 1 <= 0x7FFFFFFE )
    return StringVPrintfWorkerW(a1, a2, (size_t *)a3, a3, va);
  result = -2147024809;
  if ( a2 )
    *a1 = 0;
  return result;
}


==========

FUNCTION: memcpy_s @ 0x180008878
----------
errno_t __cdecl memcpy_s(void *const Destination, const rsize_t DestinationSize, const void *const Source, const rsize_t SourceSize)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  if ( !SourceSize )
    return 0;
  if ( !Destination )
    goto LABEL_4;
  if ( Source && DestinationSize >= SourceSize )
  {
    memcpy_0(Destination, Source, SourceSize);
    return 0;
  }
  memset_0(Destination, 0, DestinationSize);
  if ( !Source )
  {
LABEL_4:
    v8 = (errno_t *)_o__errno(Destination, DestinationSize, Source, SourceSize);
    v9 = 22;
LABEL_5:
    *v8 = v9;
    invalid_parameter_noinfo();
    return v9;
  }
  if ( DestinationSize >= SourceSize )
    return 22;
  v8 = (errno_t *)_o__errno(Destination, DestinationSize, Source, SourceSize);
  v9 = 34;
  goto LABEL_5;
}


==========

FUNCTION: StringCopyWorkerW @ 0x180008E20
----------
HRESULT __stdcall StringCopyWorkerW(STRSAFE_LPWSTR pszDest, size_t cchDest, size_t *pcchNewDestLength, STRSAFE_PCNZWCH pszSrc, size_t cchToCopy)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v5 = pszDest;
  if ( cchDest )
  {
    v7 = (char *)pszSrc - (char *)pszDest;
    do
    {
      if ( !cchToCopy )
        break;
      v8 = *(STRSAFE_LPWSTR)((char *)v5 + v7);
      if ( !v8 )
        break;
      *v5 = v8;
      --cchToCopy;
      ++v5;
      --cchDest;
    }
    while ( cchDest );
  }
  v9 = v5 - 1;
  result = cchDest == 0 ? 0x8007007A : 0;
  if ( cchDest )
    v9 = v5;
  *v9 = 0;
  return result;
}


==========

FUNCTION: StringLengthWorkerA @ 0x180008E7C
----------
HRESULT __stdcall StringLengthWorkerA(STRSAFE_PCNZCH psz, size_t cchMax, size_t *pcchLength)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v3 = 0x7FFFFFFFi64;
  do
  {
    if ( !*psz )
      break;
    ++psz;
    --v3;
  }
  while ( v3 );
  result = v3 == 0 ? 0x80070057 : 0;
  if ( !pcchLength )
    return result;
  if ( v3 )
    *pcchLength = 0x7FFFFFFF - v3;
  else
    *pcchLength = 0i64;
  return result;
}


==========

FUNCTION: StringLengthWorkerW @ 0x180008EC0
----------
HRESULT __stdcall StringLengthWorkerW(STRSAFE_PCNZWCH psz, size_t cchMax, size_t *pcchLength)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v3 = cchMax;
  do
  {
    if ( !*psz )
      break;
    ++psz;
    --cchMax;
  }
  while ( cchMax );
  result = cchMax == 0 ? 0x80070057 : 0;
  if ( !pcchLength )
    return result;
  if ( cchMax )
    *pcchLength = v3 - cchMax;
  else
    *pcchLength = 0i64;
  return result;
}


==========

FUNCTION: StringVPrintfWorkerW @ 0x180008F04
----------
HRESULT __stdcall StringVPrintfWorkerW(STRSAFE_LPWSTR pszDest, size_t cchDest, size_t *pcchNewDestLength, STRSAFE_LPCWSTR pszFormat, va_list argList)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v5 = cchDest - 1;
  v7 = 0;
  v8 = vsnwprintf(pszDest, cchDest - 1, pszFormat, argList);
  if ( v8 /*signed*/< 0 || v8 > v5 )
  {
    pszDest[v5] = 0;
    v7 = -2147024774;
  }
  else if ( v8 == v5 )
  {
    pszDest[v5] = 0;
  }
  return v7;
}


==========

FUNCTION: StringValidateDestAndLengthW @ 0x180008F64
----------
HRESULT __stdcall StringValidateDestAndLengthW(STRSAFE_LPCWSTR pszDest, size_t cchDest, size_t *pcchDestLength, const size_t cchMax)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  return StringLengthWorkerW(pszDest, 0x104ui64, pcchDestLength);
}


==========

FUNCTION: ?StringCbCopyNW@@YAJPEA_W_KPEB_W1@Z @ 0x18000CC40
----------
int __fastcall StringCbCopyNW(wchar_t *a1, unsigned __int64 a2, const wchar_t *a3, unsigned __int64 a4)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v4 = a2 >> 1;
  if ( v4 - 1 > 0x7FFFFFFE )
    return -2147024809;
  cchToCopy = a4 >> 1;
  if ( cchToCopy <= 0x7FFFFFFE )
    return StringCopyWorkerW(a1, v4, (size_t *)a3, a3, cchToCopy);
  result = -2147024809;
  *a1 = 0;
  return result;
}


==========

FUNCTION: ?StringCbCopyW@@YAJPEA_W_KPEB_W@Z @ 0x18000CC8C
----------
int __fastcall StringCbCopyW(wchar_t *a1, unsigned __int64 a2, const wchar_t *a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  v3 = a2 >> 1;
  if ( v3 - 1 <= 0x7FFFFFFE )
    return StringCopyWorkerW(a1, v3, (size_t *)a3, a3, 0x7FFFFFFEui64);
  result = -2147024809;
  if ( v3 )
    *a1 = 0;
  return result;
}


==========

FUNCTION: ?StringCchLengthA@@YAJPEBD_KPEA_K@Z @ 0x18001CB58
----------
HRESULT __fastcall StringCchLengthA(const char *a1, size_t a2, unsigned __int64 *a3)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  if ( a1 )
  {
    result = StringLengthWorkerA(a1, a2, a3);
    if ( result /*signed*/>= 0 )
      return result;
  }
  else
  {
    result = -2147024809;
  }
  if ( a3 )
    *a3 = 0i64;
  return result;
}


==========

FUNCTION: ?StringCchCopyNW@@YAJPEA_W_KPEB_W1@Z @ 0x18001DBC4
----------
HRESULT __fastcall StringCchCopyNW(wchar_t *a1, size_t a2, const wchar_t *a3, size_t cchToCopy)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  if ( a2 - 1 > 0x7FFFFFFE )
  {
    result = -2147024809;
    if ( !a2 )
      return result;
    goto LABEL_6;
  }
  if ( cchToCopy <= 0x7FFFFFFE )
    return StringCopyWorkerW(a1, a2, (size_t *)a3, a3, cchToCopy);
  result = -2147024809;
LABEL_6:
  *a1 = 0;
  return result;
}


==========

FUNCTION: memcmp_0 @ 0x1800231C0
----------
// attributes: thunk
int __cdecl memcmp_0(const void *Buf1, const void *Buf2, size_t Size)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  return memcmp(Buf1, Buf2, Size);
}


==========

FUNCTION: memcpy_0 @ 0x1800231CC
----------
// attributes: thunk
void *__cdecl memcpy_0(void *a1, const void *Src, size_t Size)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  return memcpy(a1, Src, Size);
}


==========

FUNCTION: memmove_0 @ 0x1800231D8
----------
// attributes: thunk
void *__cdecl memmove_0(void *a1, const void *Src, size_t Size)
{
  // [COLLAPSED LOCAL DECLARATIONS. PRESS KEYPAD CTRL-"+" TO EXPAND]

  return memmove(a1, Src, Size);
}


==========

