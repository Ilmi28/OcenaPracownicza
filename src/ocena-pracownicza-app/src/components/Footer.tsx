import { Box, Typography } from "@mui/material";

const Footer = () => {
    return (
        <Box
            component="footer"
            sx={{
                px: 2,
                pt: 1.5,
                pb: 0.5,
                textAlign: "center",
            }}
        >
            <Typography variant="body2" color="text.secondary">
                © {new Date().getFullYear()} Ocena Pracownicza.
            </Typography>
        </Box>
    );
};
export default Footer;
