import { createTheme } from "@mui/material/styles";

export const theme = createTheme({
    shape: {
        borderRadius: 12,
    },
    palette: {
        mode: "light",
        primary: {
            main: "#2563eb",
        },
        secondary: {
            main: "#475569",
        },
        error: {
            main: "#dc2626",
        },
        success: {
            main: "#16a34a",
        },
        warning: {
            main: "#d97706",
        },
        background: {
            default: "#f4f7fb",
            paper: "#ffffff",
        },
        text: {
            primary: "#0f172a",
            secondary: "#475569",
        },
        divider: "#e2e8f0",
    },
    typography: {
        fontFamily: ["Inter", "Segoe UI", "Roboto", "Helvetica", "Arial", "sans-serif"].join(","),
        h1: { fontWeight: 700 },
        h2: { fontWeight: 700 },
        h3: { fontWeight: 700 },
        h4: { fontWeight: 700 },
        h5: { fontWeight: 700 },
        h6: { fontWeight: 700 },
        button: { fontWeight: 600 },
    },
    components: {
        MuiCssBaseline: {
            styleOverrides: {
                html: {
                    width: "100%",
                    height: "100%",
                },
                body: {
                    width: "100%",
                    minHeight: "100%",
                    margin: 0,
                    background: "linear-gradient(180deg, #f8fbff 0%, #f4f7fb 100%)",
                },
                "#root": {
                    width: "100%",
                    minHeight: "100vh",
                },
            },
        },
        MuiButton: {
            defaultProps: {
                disableElevation: true,
            },
            styleOverrides: {
                root: {
                    borderRadius: 10,
                    textTransform: "none",
                    minHeight: 40,
                    paddingInline: 16,
                },
                containedPrimary: {
                    boxShadow: "0 6px 16px rgba(37, 99, 235, 0.20)",
                    "&:hover": {
                        boxShadow: "0 8px 20px rgba(37, 99, 235, 0.25)",
                    },
                },
            },
        },
        MuiPaper: {
            styleOverrides: {
                root: {
                    borderRadius: 14,
                    border: "1px solid #e2e8f0",
                    boxShadow: "0 10px 28px rgba(15, 23, 42, 0.06)",
                },
            },
        },
        MuiOutlinedInput: {
            styleOverrides: {
                root: {
                    backgroundColor: "#ffffff",
                },
                notchedOutline: {
                    borderColor: "#cbd5e1",
                },
            },
        },
        MuiTableHead: {
            styleOverrides: {
                root: {
                    backgroundColor: "#f8fafc",
                },
            },
        },
        MuiTableCell: {
            styleOverrides: {
                head: {
                    fontWeight: 700,
                    color: "#0f172a",
                },
            },
        },
        MuiChip: {
            styleOverrides: {
                root: {
                    borderRadius: 8,
                    fontWeight: 600,
                },
            },
        },
    },
});
