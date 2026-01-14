
import type { User } from "../Model/User";

const API_URL = "http://localhost:5000/api/User";
export const getAllUsers = async (): Promise<User[]> => {
    const response = await fetch(API_URL);
    if (!response.ok) {
        throw new Error("Failed to fetch users");
    }
    const data: User[] = await response.json();
    return data;
};

export async function AddUser(user: Omit<User, "userId">): Promise<User> {
    const response = await fetch(API_URL, {
        method: "POST",
        headers: {  
            "Content-Type": "application/json",
        },
        body: JSON.stringify(user),
    });
    if (!response.ok) {
        throw new Error("Failed to add user");
    }
    const data: User = await response.json();
    return data;
}


