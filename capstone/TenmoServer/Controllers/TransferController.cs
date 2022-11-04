﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.DAO;
using TenmoServer.Controllers;
using TenmoServer.Models;
using TenmoServer.Security;

namespace TenmoServer.Controllers
{

    [Route("user")]
    [ApiController]

    public class TransferController : ControllerBase
    {
        private readonly IUserDao userDao;
        private ITransferDao transferDao;
        public TransferController(ITransferDao transferDao)
        {
            this.transferDao = transferDao;
        }

        [HttpGet("{username}")]
        public ActionResult<decimal> GetBalance(string username)
        {
            decimal balance = transferDao.GetBalance(username);

            if (balance >= 0)
            {
                return balance;
            }
            else
            {
                return NotFound();
            }

        }
        [HttpPut("{id}")] // /users/:id 
        public ActionResult<Account> UpdateBalance(int accountId)
        {
            Account fromAccount = transferDao.GetAccount(accountId);
            if (fromAccount == null) //if the user doesn't exist 
            {
                return NotFound(); //404
            }

            //do what you gotta do to update the thing

            Account updatedAccount = transferDao.UpdateAccount(fromAccount);
            //have to somehow update updatedUser's balance by using Math method
            return updatedAccount;
        }
        [HttpPost("transfer")]
        public Transfer SendTransfer(Transfer incomingTransfer)
        {
            //Transfer fromUser = userDao.GetUser(incomingTransfer.FromAccountId);
            //User toUser = userDao.GetUser(incomingTransfer.ToAccountId);
            //Transfer Amount
            Transfer transfer = new Transfer();

            transfer.TransferStatusId = 2;

            //retrieve balance from first user from database, subtract amount from first user, add amount to second user, show balance of first userdecimel


            decimal fromUserInitialBalance = transferDao.GetBalanceByAccount(incomingTransfer.FromAccountId);
            Account fromAccount = transferDao.GetAccount(incomingTransfer.FromAccountId);
            Account toAccount = transferDao.GetAccount(incomingTransfer.ToAccountId);

            if (fromUserInitialBalance >= incomingTransfer.TransferAmount)
            {
                
                fromAccount.Balance = fromUserInitialBalance - incomingTransfer.TransferAmount;
                toAccount.Balance = transferDao.GetBalanceByAccount(incomingTransfer.ToAccountId) + incomingTransfer.TransferAmount;
                transferDao.UpdateAccount(fromAccount);
                transferDao.UpdateAccount(toAccount);
                transfer.TransferAmount = incomingTransfer.TransferAmount;

            }
            else
            {
                transfer.TransferStatusId = 3;
            }
            //AddTransfer(transfer, fromUser.Username, toUser.Username);
            return transfer;

        }
        [HttpGet("account/{username}")]
        public ActionResult<int> GetAccountIdFromDao(string username)
        {
            int accountId = transferDao.GetAccountId(username);
            if(accountId != 0)
            {
                return accountId;
            }
            else
            {
                return NotFound();
            }
        }


        [HttpPost()]
        public ActionResult<Transfer> AddTransfer(Transfer transfer, string fromUser, string toUser)
        {
            Transfer added = transferDao.CreateTransfer(transfer, fromUser, toUser);
            return Created($"/user/{added.TransferId}", added);
        }
        //STEPS FOR SENDING MONEY
        // 1) call method that gets user balance
        // 2) call method that does the math
        // 3) call method that updates the users' balances
        [HttpGet()]
        public ActionResult<List<User>> GetUsers()
        {
            List<User> users = transferDao.GetUsers();

            if (users.Count >= 0)
            {
                return users;
            }
            else
            {
                return NotFound();
            }

        }
    }
}

