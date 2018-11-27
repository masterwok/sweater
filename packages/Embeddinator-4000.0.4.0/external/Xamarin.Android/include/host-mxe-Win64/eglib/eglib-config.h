#ifndef __EGLIB_CONFIG_H
#define __EGLIB_CONFIG_H

/*
 * System-dependent settings
 */
#define G_GNUC_PRETTY_FUNCTION   
#define G_GNUC_UNUSED            __attribute__((__unused__))
#define G_BYTE_ORDER             G_LITTLE_ENDIAN
#define G_GNUC_NORETURN          __attribute__((__noreturn__))
#define G_SEARCHPATH_SEPARATOR_S ";"
#define G_SEARCHPATH_SEPARATOR   ';'
#define G_DIR_SEPARATOR          '\\'
#define G_DIR_SEPARATOR_S        "\\"
#define G_BREAKPOINT()           G_STMT_START { __asm__ ("int $03"); } G_STMT_END
#define G_OS_WIN32
#define GPOINTER_TO_INT(ptr)   ((gint)(long long) (ptr))
#define GPOINTER_TO_UINT(ptr)  ((guint)(unsigned long long) (ptr))
#define GINT_TO_POINTER(v)     ((gpointer)(long long) (v))
#define GUINT_TO_POINTER(v)    ((gpointer)(unsigned long long) (v))

#if 0 == 1
#define G_HAVE_ALLOCA_H
#endif

typedef unsigned long long gsize;
typedef signed   long long gssize;

#define G_GSIZE_FORMAT   "I64u"

#if 1 == 1
#define G_HAVE_ISO_VARARGS
#endif

#if defined (__native_client__) || defined (HOST_WATCHOS)
#undef G_BREAKPOINT
#define G_BREAKPOINT()
#endif

typedef void * GPid;

#endif
