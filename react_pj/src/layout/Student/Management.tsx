// render table ra tu api 
import React, { useEffect, useState } from "react";
import { getAllUsers } from "../../Api/UserApi";
import type { User } from "../../Model/User";
import { 
    Table, TableBody, TableCell, TableHead, TableRow, Paper, TableContainer, Button, TextField, IconButton
} from "@mui/material";
import { Edit, Delete, Save, Cancel } from "@mui/icons-material";
import { FadeModal } from "../../utils/FadeModal";

const Management: React.FC = () => {
    const [users, setUsers] = useState<User[]>([]);
    const [loading, setLoading] = useState<boolean>(true);
    const [error, setError] = useState<string | null>(null);
    const [openDialog, setOpenDialog] = useState<boolean>(false);
    const [editingUser, setEditingUser] = useState<User | null>(null);
    const [openAddDialog, setOpenAddDialog] = useState<boolean>(false);
    const [newUser, setNewUser] = useState<Partial<User>>({ username: '', email: '' });

    useEffect(() => {
        const fetchUsers = async () => {
            try {
                const data = await getAllUsers();
                setUsers(data);
            }
            catch (err) {
                setError((err as Error).message);
            }
            finally {
                setLoading(false);
            }
        };
        fetchUsers();
    }, []);

    if (loading) {
        return <div>Loading...</div>;
    }

    if (error) {
        return <div>Error: {error}</div>;
    }

    const handleOpenEdit = (user: User) => {
        setEditingUser({ ...user });
        setOpenDialog(true);
    };

    const handleCloseDialog = () => {
        setOpenDialog(false);
        setEditingUser(null);
    };

    const handleOpenAdd = () => {
        setNewUser({ username: '', email: '' });
        setOpenAddDialog(true);
    };

    const handleCloseAddDialog = () => {
        setOpenAddDialog(false);
        setNewUser({ username: '', email: '' });
    };

    const handleNewUserChange = (field: keyof User, value: string) => {
        setNewUser({ ...newUser, [field]: value });
    };

    const handleAddUser = () => {
        if (newUser.username && newUser.email) {
            const userToAdd: User = {
                userId: Math.max(...users.map(u => u.userId), 0) + 1,
                username: newUser.username,
                email: newUser.email,
                createdAt: new Date().toISOString()
            };
            setUsers([...users, userToAdd]);
            handleCloseAddDialog();
            // TODO: Gọi API để thêm
            console.log("Adding user:", userToAdd);
        }
    };

    const handleInputChange = (field: keyof User, value: string) => {
        if (editingUser) {
            setEditingUser({ ...editingUser, [field]: value });
        }
    };

    const handleSave = () => {
        if (editingUser) {
            setUsers(users.map(u => u.userId === editingUser.userId ? editingUser : u));
            handleCloseDialog();
            fetch(`/api/users/${editingUser.userId}`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(editingUser),
            })
            .then(response => {
                if (!response.ok) {     
                    throw new Error('Network response was not ok');
                }
                return response.json();
            })
            .then(data => {
                console.log('Success:', data);
            })
            .catch((error) => {
                console.error('Error:', error);
            });

        }
    };

    const handleDelete = (userId: number) => {
        if (window.confirm("Bạn có chắc muốn xóa sinh viên này?")) {
            setUsers(users.filter(u => u.userId !== userId));
            // TODO: Gọi API để xóa
            console.log("Deleting user:", userId);
        }
    };

    return (
        <>
            <TableContainer component={Paper} style={{ maxWidth: 800, margin: '20px auto', padding: '20px' }}>
                <Button variant="contained" color="primary" style={{ margin: '10px' }} onClick={handleOpenAdd}>
                    Thêm Sinh Viên
                </Button>
                <Table>
                    <TableHead>
                        <TableRow>
                            <TableCell>User ID</TableCell>
                            <TableCell>Username</TableCell>
                            <TableCell>Email</TableCell>
                            <TableCell>Created At</TableCell>
                            <TableCell>Hành Động</TableCell>
                        </TableRow>
                    </TableHead>    
                    <TableBody>
                        {users.map((user) => (
                            <TableRow key={user.userId}>
                                <TableCell>{user.userId}</TableCell>
                                <TableCell>{user.username}</TableCell>
                                <TableCell>{user.email}</TableCell>
                                <TableCell>{new Date(user.createdAt).toLocaleDateString()}</TableCell>
                                <TableCell>
                                    <IconButton 
                                        size="small" 
                                        color="primary"
                                        onClick={() => handleOpenEdit(user)}
                                        title="Sửa"
                                    >
                                        <Edit />
                                    </IconButton>
                                    <IconButton 
                                        size="small" 
                                        color="error"
                                        onClick={() => handleDelete(user.userId)}
                                        title="Xóa"
                                    >
                                        <Delete />
                                    </IconButton>
                                </TableCell>
                            </TableRow>
                        ))}
                    </TableBody>
                </Table>
            </TableContainer>

            <FadeModal 
                open={openDialog} 
                handleClose={handleCloseDialog}
                title="Chỉnh Sửa Thông Tin Sinh Viên"
            >
                {editingUser && (
                    <div>
                        <TextField
                            fullWidth
                            label="User ID"
                            value={editingUser.userId}
                            disabled
                            variant="outlined"
                            margin="normal"
                            size="small"
                        />
                        <TextField
                            fullWidth
                            label="Username"
                            value={editingUser.username}
                            onChange={(e) => handleInputChange('username', e.target.value)}
                            variant="outlined"
                            margin="normal"
                            size="small"
                        />
                        <TextField
                            fullWidth
                            label="Email"
                            value={editingUser.email}
                            onChange={(e) => handleInputChange('email', e.target.value)}
                            variant="outlined"
                            margin="normal"
                            size="small"
                            type="email"
                        />
                        <TextField
                            fullWidth
                            label="Created At"
                            value={new Date(editingUser.createdAt).toLocaleString()}
                            disabled
                            variant="outlined"
                            margin="normal"
                            size="small"
                        />
                        <div style={{ display: 'flex', gap: '12px', marginTop: '24px', justifyContent: 'flex-end' }}>
                            <Button 
                                variant="outlined" 
                                color="secondary"
                                startIcon={<Cancel />}
                                onClick={handleCloseDialog}
                            >
                                Hủy
                            </Button>
                            <Button 
                                variant="contained" 
                                color="primary"
                                startIcon={<Save />}
                                onClick={handleSave}
                            >
                                Lưu
                            </Button>
                        </div>
                    </div>
                )}
            </FadeModal>

            <FadeModal 
                open={openAddDialog} 
                handleClose={handleCloseAddDialog}
                title="Thêm Sinh Viên Mới"
            >
                <div>
                    <TextField
                        fullWidth
                        label="Username"
                        value={newUser.username || ''}
                        onChange={(e) => handleNewUserChange('username', e.target.value)}
                        variant="outlined"
                        margin="normal"
                        size="small"
                        required
                    />
                    <TextField
                        fullWidth
                        label="Email"
                        value={newUser.email || ''}
                        onChange={(e) => handleNewUserChange('email', e.target.value)}
                        variant="outlined"
                        margin="normal"
                        size="small"
                        type="email"
                        required
                    />
                    <div style={{ display: 'flex', gap: '12px', marginTop: '24px', justifyContent: 'flex-end' }}>
                        <Button 
                            variant="outlined" 
                            color="secondary"
                            startIcon={<Cancel />}
                            onClick={handleCloseAddDialog}
                        >
                            Hủy
                        </Button>
                        <Button 
                            variant="contained" 
                            color="primary"
                            startIcon={<Save />}
                            onClick={handleAddUser}
                            disabled={!newUser.username || !newUser.email}
                        >
                            Thêm
                        </Button>
                    </div>
                </div>
            </FadeModal>
        </>
    );
}

export default Management;
