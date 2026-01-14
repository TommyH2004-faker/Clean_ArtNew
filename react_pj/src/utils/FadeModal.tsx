import * as React from "react";
import Backdrop from "@mui/material/Backdrop";
import Box from "@mui/material/Box";
import Modal from "@mui/material/Modal";
import Fade from "@mui/material/Fade";
import IconButton from "@mui/material/IconButton";
import CloseIcon from "@mui/icons-material/Close";

const style = {
	position: "absolute" as const,
	top: "50%",
	left: "50%",
	transform: "translate(-50%, -50%)",
	width: "90%",
	maxWidth: "600px",
	maxHeight: "90vh",
	overflowY: "auto",
	bgcolor: "background.paper",
	borderRadius: 2,
	boxShadow: 24,
};

interface FadeModalProps {
	open: boolean;
	handleClose: () => void;
	title?: string;
	children: React.ReactNode;
}

export const FadeModal: React.FC<FadeModalProps> = ({ open, handleClose, title, children }) => {
	return (
		<Modal
			aria-labelledby='transition-modal-title'
			aria-describedby='transition-modal-description'
			open={open}
			onClose={handleClose}
			closeAfterTransition
			slots={{ backdrop: Backdrop }}
			slotProps={{
				backdrop: {
					timeout: 500,
				},
			}}
		>
			<Fade in={open}>
				<Box sx={style}>
					{title && (
						<Box
							sx={{
								display: "flex",
								justifyContent: "space-between",
								alignItems: "center",
								px: 3,
								py: 2,
								bgcolor: "primary.main",
								color: "white",
								borderTopLeftRadius: 8,
								borderTopRightRadius: 8,
							}}
						>
							<h2 style={{ margin: 0, fontSize: "1.25rem", fontWeight: 600 }}>{title}</h2>
							<IconButton onClick={handleClose} size="small" sx={{ color: "white" }}>
								<CloseIcon />
							</IconButton>
						</Box>
					)}
					<Box sx={{ p: 3 }}>{children}</Box>
				</Box>
			</Fade>
		</Modal>
	);
};
